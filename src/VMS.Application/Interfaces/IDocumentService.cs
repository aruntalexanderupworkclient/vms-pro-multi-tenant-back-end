using Microsoft.AspNetCore.Http;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Documents;

namespace VMS.Application.Interfaces;

public interface IDocumentService
{
    Task<ApiResponse<PagedResult<DocumentDto>>> GetAllAsync(Guid? entityId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse<DocumentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(byte[] Data, string FileName, string ContentType)?> DownloadAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<DocumentDto>> UploadAsync(IFormFile file, UploadDocumentDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
