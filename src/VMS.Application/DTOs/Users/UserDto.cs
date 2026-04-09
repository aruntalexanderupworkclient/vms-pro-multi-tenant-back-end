namespace VMS.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsActive { get; set; }
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
}
