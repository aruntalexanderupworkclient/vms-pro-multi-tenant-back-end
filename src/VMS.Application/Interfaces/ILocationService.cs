using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Locations;

namespace VMS.Application.Interfaces;

public interface ILocationService
{
    Task<ApiResponse<List<LocationDto>>> GetTreeAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<LocationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<LocationDto>> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<LocationDto>> UpdateAsync(Guid id, UpdateLocationDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
