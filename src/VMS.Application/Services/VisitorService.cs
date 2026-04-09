using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Visitors;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class VisitorService : IVisitorService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public VisitorService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<VisitorDto>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<Visitor>().Query();
        var totalCount = await query.CountAsync(cancellationToken);
        var visitors = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ApiResponse<PagedResult<VisitorDto>>.SuccessResponse(new PagedResult<VisitorDto>
        {
            Items = _mapper.Map<List<VisitorDto>>(visitors),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<VisitorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visitor = await _uow.Repository<Visitor>().Query()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (visitor == null)
            return ApiResponse<VisitorDto>.FailResponse("Visitor not found.", "VISITOR_NOT_FOUND");

        return ApiResponse<VisitorDto>.SuccessResponse(_mapper.Map<VisitorDto>(visitor));
    }

    public async Task<ApiResponse<VisitorDto>> CreateAsync(CreateVisitorDto dto, CancellationToken cancellationToken = default)
    {
        var visitor = _mapper.Map<Visitor>(dto);

        await _uow.Repository<Visitor>().AddAsync(visitor, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return ApiResponse<VisitorDto>.SuccessResponse(_mapper.Map<VisitorDto>(visitor), "Visitor created successfully.");
    }

    public async Task<ApiResponse<VisitorDto>> UpdateAsync(Guid id, UpdateVisitorDto dto, CancellationToken cancellationToken = default)
    {
        var visitor = await _uow.Repository<Visitor>().GetByIdAsync(id, cancellationToken);

        if (visitor == null)
            return ApiResponse<VisitorDto>.FailResponse("Visitor not found.", "VISITOR_NOT_FOUND");

        if (dto.FullName != null) visitor.FullName = dto.FullName;
        if (dto.Email != null) visitor.Email = dto.Email;
        if (dto.Phone != null) visitor.Phone = dto.Phone;
        if (dto.Company != null) visitor.Company = dto.Company;
        if (dto.IdProofType != null) visitor.IdProofType = dto.IdProofType;
        if (dto.IdProofNumber != null) visitor.IdProofNumber = dto.IdProofNumber;
        if (dto.IsBlacklisted.HasValue) visitor.IsBlacklisted = dto.IsBlacklisted.Value;

        _uow.Repository<Visitor>().Update(visitor);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<VisitorDto>.SuccessResponse(_mapper.Map<VisitorDto>(visitor), "Visitor updated successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var visitor = await _uow.Repository<Visitor>().GetByIdAsync(id, cancellationToken);
        if (visitor == null)
            return ApiResponse.FailResponse("Visitor not found.", "VISITOR_NOT_FOUND");

        var hasActiveVisits = await _uow.Repository<Visit>().AnyAsync(
            v => v.VisitorId == id && (v.Status == VMS.Core.Enums.VisitStatus.Scheduled || v.Status == VMS.Core.Enums.VisitStatus.CheckedIn),
            cancellationToken);

        if (hasActiveVisits)
            return ApiResponse.FailResponse("Cannot delete visitor with active visits.", "VISITOR_HAS_ACTIVE_VISITS");

        _uow.Repository<Visitor>().SoftDelete(visitor);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResponse("Visitor deleted successfully.");
    }
}
