namespace VMS.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? TenantId { get; }
    Guid? RoleId { get; }
    bool IsAdmin { get; }
    string? Email { get; }
}
