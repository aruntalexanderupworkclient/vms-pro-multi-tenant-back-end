namespace VMS.Core.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsAdmin { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public Tenant? Tenant { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
