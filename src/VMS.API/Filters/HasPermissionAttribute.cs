using Microsoft.AspNetCore.Authorization;

namespace VMS.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission_";

    public HasPermissionAttribute(string menu, string permission)
        : base($"{PolicyPrefix}{menu}_{permission}")
    {
        Menu = menu;
        Permission = permission;
    }

    public string Menu { get; }
    public string Permission { get; }
}
