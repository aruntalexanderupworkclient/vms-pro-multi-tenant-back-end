using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Hosts;

namespace VMS.Application.Interfaces;

public interface IHostPersonService
{
    Task<ApiResponse<PagedResult<HostPersonDto>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse<HostPersonDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<HostPersonDto>> CreateAsync(CreateHostPersonDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<HostPersonDto>> UpdateAsync(Guid id, UpdateHostPersonDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
