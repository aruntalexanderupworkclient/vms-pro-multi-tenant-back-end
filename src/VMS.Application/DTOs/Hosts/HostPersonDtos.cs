namespace VMS.Application.DTOs.Hosts;

public class HostPersonDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public Guid? UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateHostPersonDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? UserId { get; set; }
}

public class UpdateHostPersonDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? UserId { get; set; }
    public bool? IsActive { get; set; }
}
