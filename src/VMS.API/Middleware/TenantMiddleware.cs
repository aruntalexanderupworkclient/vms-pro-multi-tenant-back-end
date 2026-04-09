using System.Security.Claims;
using VMS.Application.Interfaces;
using VMS.Shared.Constants;

namespace VMS.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst(AppConstants.ClaimTypes.TenantId)?.Value;
            if (Guid.TryParse(tenantClaim, out var tenantId))
            {
                tenantProvider.SetTenantId(tenantId);
            }
        }

        await _next(context);
    }
}
