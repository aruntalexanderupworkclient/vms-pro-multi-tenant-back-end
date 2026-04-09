using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VMS.Application.Interfaces;
using VMS.Core.Interfaces;
using VMS.Infrastructure.Data;
using VMS.Infrastructure.Repositories;
using VMS.Infrastructure.Services;
using VMS.Infrastructure.Services.Notifications;

namespace VMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var provider = configuration["DatabaseProvider"] ?? "InMemory";

        if (provider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<VmsDbContext>((sp, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });
        }
        else
        {
            services.AddDbContext<VmsDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("VmsDb");
            });
        }

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Tenant
        services.AddScoped<ITenantProvider, TenantProvider>();

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        // Notification channels (Strategy Pattern)
        services.AddScoped<INotificationChannel, EmailNotificationChannel>();
        services.AddScoped<INotificationChannel, SmsNotificationChannel>();
        services.AddScoped<INotificationChannel, WhatsAppNotificationChannel>();
        services.AddScoped<INotificationChannel, InAppNotificationChannel>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

        return services;
    }
}
