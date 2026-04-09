using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VMS.Infrastructure.Data;
using VMS.Shared.Constants;

namespace VMS.API.Filters;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Menu { get; }
    public string Permission { get; }

    public PermissionRequirement(string menu, string permission)
    {
        Menu = menu;
        Permission = permission;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var isAdminClaim = context.User.FindFirst(AppConstants.ClaimTypes.IsAdmin)?.Value;
        if (bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin)
        {
            context.Succeed(requirement);
            return;
        }

        var roleIdClaim = context.User.FindFirst(AppConstants.ClaimTypes.RoleId)?.Value;
        var tenantIdClaim = context.User.FindFirst(AppConstants.ClaimTypes.TenantId)?.Value;

        if (!Guid.TryParse(roleIdClaim, out var roleId) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            context.Fail();
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VmsDbContext>();

        var hasPermission = await dbContext.RolePermissions
            .IgnoreQueryFilters()
            .AnyAsync(rp =>
                rp.RoleId == roleId &&
                rp.TenantId == tenantId &&
                !rp.IsDeleted &&
                rp.Menu != null &&
                rp.Menu.Name == requirement.Menu &&
                (
                    (requirement.Permission == "CanCreate" && rp.CanCreate) ||
                    (requirement.Permission == "CanRead" && rp.CanRead) ||
                    (requirement.Permission == "CanUpdate" && rp.CanUpdate) ||
                    (requirement.Permission == "CanDelete" && rp.CanDelete) ||
                    (requirement.Permission == "CanPrint" && rp.CanPrint)
                ));

        if (hasPermission)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(HasPermissionAttribute.PolicyPrefix))
        {
            var parts = policyName[HasPermissionAttribute.PolicyPrefix.Length..].Split('_', 2);
            if (parts.Length == 2)
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(parts[0], parts[1]))
                    .Build();
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }
        }

        return _fallbackProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();
}
