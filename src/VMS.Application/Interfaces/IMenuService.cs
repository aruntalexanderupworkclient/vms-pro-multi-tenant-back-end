using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Menus;

namespace VMS.Application.Interfaces;

public interface IMenuService
{
    Task<ApiResponse<List<MenuDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MenuDto>>> GetTreeAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MenuWithPermissionDto>>> GetMenusByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<MenuWithPermissionDto>>> GetMyMenusAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<MenuDto>> CreateAsync(CreateMenuDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<MenuDto>> UpdateAsync(Guid id, UpdateMenuDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
