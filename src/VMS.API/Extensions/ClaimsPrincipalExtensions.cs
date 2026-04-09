using System.Security.Claims;
using VMS.Shared.Constants;

namespace VMS.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(AppConstants.ClaimTypes.UserId)?.Value
                    ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    public static Guid GetTenantId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(AppConstants.ClaimTypes.TenantId)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    public static Guid? GetRoleId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(AppConstants.ClaimTypes.RoleId)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(AppConstants.ClaimTypes.IsAdmin)?.Value;
        return bool.TryParse(claim, out var isAdmin) && isAdmin;
    }

    public static string? GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value;
    }

    public static Dictionary<string, string> GetAllClaims(this ClaimsPrincipal principal)
    {
        return principal.Claims.ToDictionary(c => c.Type, c => c.Value);
    }
}
