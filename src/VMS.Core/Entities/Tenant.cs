namespace VMS.Core.Entities;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TenantTypeId { get; set; }
    public Guid? PlanTypeId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public MdmTenantType? TenantType { get; set; }
    public MdmPlanType? PlanType { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<HostPerson> Hosts { get; set; } = new List<HostPerson>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
