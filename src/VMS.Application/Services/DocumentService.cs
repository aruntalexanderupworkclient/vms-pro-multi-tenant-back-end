using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Documents;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;

    public DocumentService(IUnitOfWork uow, IMapper mapper, ITenantProvider tenantProvider)
    {
        _uow = uow;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
    }

    public async Task<ApiResponse<PagedResult<DocumentDto>>> GetAllAsync(Guid? entityId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<Document>().Query()
            .Include(d => d.EntityType)
            .Include(d => d.FileType)
            .AsQueryable();

        if (entityId.HasValue)
            query = query.Where(d => d.EntityId == entityId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var docs = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ApiResponse<PagedResult<DocumentDto>>.SuccessResponse(new PagedResult<DocumentDto>
        {
            Items = _mapper.Map<List<DocumentDto>>(docs),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<DocumentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doc = await _uow.Repository<Document>().Query()
            .Include(d => d.EntityType)
            .Include(d => d.FileType)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doc == null)
            return ApiResponse<DocumentDto>.FailResponse("Document not found.", "DOC_NOT_FOUND");

        return ApiResponse<DocumentDto>.SuccessResponse(_mapper.Map<DocumentDto>(doc));
    }

    public async Task<(byte[] Data, string FileName, string ContentType)?> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doc = await _uow.Repository<Document>().Query()
            .Include(d => d.Blob)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doc?.Blob == null)
            return null;

        return (doc.Blob.Data, doc.OriginalFileName ?? doc.FileName, doc.ContentType ?? "application/octet-stream");
    }

    public async Task<ApiResponse<DocumentDto>> UploadAsync(IFormFile file, UploadDocumentDto dto, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return ApiResponse<DocumentDto>.FailResponse("No file provided.", "DOC_NO_FILE");

        byte[] fileData;
        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms, cancellationToken);
            fileData = ms.ToArray();
        }

        var document = new Document
        {
            FileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
            OriginalFileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            EntityTypeId = dto.EntityTypeId,
            FileTypeId = dto.FileTypeId,
            EntityId = dto.EntityId,
            Description = dto.Description,
            TenantId = _tenantProvider.GetTenantId()
        };

        await _uow.Repository<Document>().AddAsync(document, cancellationToken);

        document.Blob = new DocumentBlob
        {
            DocumentId = document.Id,
            Data = fileData
        };

        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<DocumentDto>.SuccessResponse(_mapper.Map<DocumentDto>(document), "Document uploaded successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var doc = await _uow.Repository<Document>().GetByIdAsync(id, cancellationToken);
        if (doc == null)
            return ApiResponse.FailResponse("Document not found.", "DOC_NOT_FOUND");

        _uow.Repository<Document>().SoftDelete(doc);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResponse("Document deleted successfully.");
    }
}
