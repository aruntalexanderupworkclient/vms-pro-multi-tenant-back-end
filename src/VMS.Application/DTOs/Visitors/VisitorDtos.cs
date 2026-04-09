using VMS.Core.Enums;

namespace VMS.Application.DTOs.Visitors;

public class VisitorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? PhotoUrl { get; set; }
    public string? IdProofType { get; set; }
    public string? IdProofNumber { get; set; }
    public bool IsBlacklisted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVisitorDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? IdProofType { get; set; }
    public string? IdProofNumber { get; set; }
}

public class UpdateVisitorDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? IdProofType { get; set; }
    public string? IdProofNumber { get; set; }
    public bool? IsBlacklisted { get; set; }
}

public class VisitDto
{
    public Guid Id { get; set; }
    public Guid VisitorId { get; set; }
    public string? VisitorName { get; set; }
    public Guid? HostId { get; set; }
    public string? HostName { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public string? Purpose { get; set; }
    public VisitStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? QRCode { get; set; }
    public string? AccessCardNumber { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateVisitDto
{
    public Guid VisitorId { get; set; }
    public Guid? HostId { get; set; }
    public Guid? LocationId { get; set; }
    public string? Purpose { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public string? Remarks { get; set; }
}

public class CheckInDto
{
    public string? AccessCardNumber { get; set; }
}

public class CheckOutDto
{
    public string? Remarks { get; set; }
}
