using Microsoft.Extensions.DependencyInjection;
using VMS.Application.Interfaces;
using VMS.Application.Mappings;
using VMS.Application.Services;

namespace VMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<IVisitService, VisitService>();
        services.AddScoped<IHostPersonService, HostPersonService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<INotificationService, NotificationAppService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IMdmService, MdmService>();
        services.AddScoped<IMenuService, MenuService>();

        return services;
    }
}
