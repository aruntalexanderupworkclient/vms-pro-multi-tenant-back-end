namespace VMS.Core.Entities;

public class Visitor : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? PhotoUrl { get; set; }
    public string? IdProofType { get; set; }
    public string? IdProofNumber { get; set; }
    public bool IsBlacklisted { get; set; } = false;

    public Tenant? Tenant { get; set; }
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
