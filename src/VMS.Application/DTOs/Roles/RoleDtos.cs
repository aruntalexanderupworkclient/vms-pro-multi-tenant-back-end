namespace VMS.Application.DTOs.Roles;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    public List<RolePermissionDto> Permissions { get; set; } = new();
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAdmin { get; set; } = false;
}

public class UpdateRoleDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsAdmin { get; set; }
    public bool? IsActive { get; set; }
}

public class RolePermissionDto
{
    public Guid MenuId { get; set; }
    public string? MenuName { get; set; }
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanPrint { get; set; }
}

public class SetRolePermissionsDto
{
    public List<RolePermissionDto> Permissions { get; set; } = new();
}
