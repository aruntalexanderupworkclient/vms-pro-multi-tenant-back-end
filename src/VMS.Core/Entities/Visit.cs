using VMS.Core.Enums;

namespace VMS.Core.Entities;

public class Visit : BaseEntity
{
    public Guid VisitorId { get; set; }
    public Guid? HostId { get; set; }
    public Guid? LocationId { get; set; }
    public string? Purpose { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? QRCode { get; set; }
    public string? AccessCardNumber { get; set; }
    public DateTime? AccessCardIssuedAt { get; set; }
    public DateTime? AccessCardReturnedAt { get; set; }
    public string? Remarks { get; set; }
    public DateTime ScheduledDateTime { get; set; }

    public Visitor? Visitor { get; set; }
    public HostPerson? Host { get; set; }
    public Location? Location { get; set; }
}
