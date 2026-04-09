namespace VMS.Core.Entities;

public class HostPerson : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? UserId { get; set; }
    public bool IsActive { get; set; } = true;

    public Location? Location { get; set; }
    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
