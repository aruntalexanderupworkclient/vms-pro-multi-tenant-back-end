using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Visitors;

namespace VMS.Application.Interfaces;

public interface IVisitService
{
    Task<ApiResponse<PagedResult<VisitDto>>> GetAllAsync(Guid? visitorId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse<VisitDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<VisitDto>> CreateAsync(CreateVisitDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<VisitDto>> CheckInAsync(Guid visitId, CheckInDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<VisitDto>> CheckOutAsync(Guid visitId, CheckOutDto dto, CancellationToken cancellationToken = default);
}
