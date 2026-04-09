using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using VMS.Application.Interfaces;
using VMS.Shared.Constants;

namespace VMS.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var claim = User?.FindFirst(AppConstants.ClaimTypes.UserId)?.Value
                        ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public Guid? TenantId
    {
        get
        {
            var claim = User?.FindFirst(AppConstants.ClaimTypes.TenantId)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public Guid? RoleId
    {
        get
        {
            var claim = User?.FindFirst(AppConstants.ClaimTypes.RoleId)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public bool IsAdmin
    {
        get
        {
            var claim = User?.FindFirst(AppConstants.ClaimTypes.IsAdmin)?.Value;
            return bool.TryParse(claim, out var isAdmin) && isAdmin;
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
}
