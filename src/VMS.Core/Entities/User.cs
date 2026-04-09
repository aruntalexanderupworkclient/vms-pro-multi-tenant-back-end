namespace VMS.Core.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? Phone { get; set; }
    public Guid? RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? GoogleId { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public Role? Role { get; set; }
    public Tenant? Tenant { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
