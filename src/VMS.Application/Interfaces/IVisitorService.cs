using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Visitors;

namespace VMS.Application.Interfaces;

public interface IVisitorService
{
    Task<ApiResponse<PagedResult<VisitorDto>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse<VisitorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<VisitorDto>> CreateAsync(CreateVisitorDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<VisitorDto>> UpdateAsync(Guid id, UpdateVisitorDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
