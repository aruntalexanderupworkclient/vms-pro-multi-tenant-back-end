using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Roles;

namespace VMS.Application.Interfaces;

public interface IRoleService
{
    Task<ApiResponse<List<RoleDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<RoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<RoleDto>> UpdateAsync(Guid id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse> SetPermissionsAsync(Guid roleId, SetRolePermissionsDto dto, CancellationToken cancellationToken = default);
}
