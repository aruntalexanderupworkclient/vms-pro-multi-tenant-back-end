using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Roles;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RoleService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<RoleDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _uow.Repository<Role>().Query()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Menu)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<RoleDto>>.SuccessResponse(_mapper.Map<List<RoleDto>>(roles));
    }

    public async Task<ApiResponse<RoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _uow.Repository<Role>().Query()
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Menu)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return ApiResponse<RoleDto>.FailResponse("Role not found.", "ROLE_NOT_FOUND");

        return ApiResponse<RoleDto>.SuccessResponse(_mapper.Map<RoleDto>(role));
    }

    public async Task<ApiResponse<RoleDto>> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _uow.Repository<Role>().AnyAsync(r => r.Name == dto.Name, cancellationToken);
        if (exists)
            return ApiResponse<RoleDto>.FailResponse("Role name already exists.", "ROLE_NAME_EXISTS");

        var role = _mapper.Map<Role>(dto);
        await _uow.Repository<Role>().AddAsync(role, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<RoleDto>.SuccessResponse(_mapper.Map<RoleDto>(role), "Role created successfully.");
    }

    public async Task<ApiResponse<RoleDto>> UpdateAsync(Guid id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _uow.Repository<Role>().GetByIdAsync(id, cancellationToken);
        if (role == null)
            return ApiResponse<RoleDto>.FailResponse("Role not found.", "ROLE_NOT_FOUND");

        if (dto.Name != null) role.Name = dto.Name;
        if (dto.Description != null) role.Description = dto.Description;
        if (dto.IsAdmin.HasValue) role.IsAdmin = dto.IsAdmin.Value;
        if (dto.IsActive.HasValue) role.IsActive = dto.IsActive.Value;

        _uow.Repository<Role>().Update(role);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<RoleDto>.SuccessResponse(_mapper.Map<RoleDto>(role), "Role updated successfully.");
    }

    public async Task<ApiResponse> SetPermissionsAsync(Guid roleId, SetRolePermissionsDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _uow.Repository<Role>().Query()
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role == null)
            return ApiResponse.FailResponse("Role not found.", "ROLE_NOT_FOUND");

        foreach (var existing in role.RolePermissions.ToList())
        {
            _uow.Repository<RolePermission>().Remove(existing);
        }

        foreach (var permDto in dto.Permissions)
        {
            var perm = new RolePermission
            {
                RoleId = role.Id,
                MenuId = permDto.MenuId,
                TenantId = role.TenantId,
                CanCreate = permDto.CanCreate,
                CanRead = permDto.CanRead,
                CanUpdate = permDto.CanUpdate,
                CanDelete = permDto.CanDelete,
                CanPrint = permDto.CanPrint
            };
            await _uow.Repository<RolePermission>().AddAsync(perm, cancellationToken);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return ApiResponse.SuccessResponse("Permissions updated successfully.");
    }
}
