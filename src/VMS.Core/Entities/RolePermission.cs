namespace VMS.Core.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid MenuId { get; set; }
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanPrint { get; set; }

    public Role? Role { get; set; }
    public Menu? Menu { get; set; }
}
