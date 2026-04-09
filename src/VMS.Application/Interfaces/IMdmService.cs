using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Mdm;

namespace VMS.Application.Interfaces;

public interface IMdmService
{
    Task<ApiResponse<List<MdmItemDto>>> GetTenantTypesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MdmItemDto>>> GetPlanTypesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MdmItemDto>>> GetLocationTypesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MdmItemDto>>> GetFileTypesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MdmItemDto>>> GetEntityTypesAsync(CancellationToken cancellationToken = default);
}
