using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Order.Infrastructure.Persistence.Interceptors;
using Order.Infrastructure.Persistence.Seeds;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        var connectionString = configuration.GetConnectionString("Database");

        // 注册审计拦截器
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DomainEventsDispatchInterceptor>();

        // Add-Migration InitialOrderDb -OutputDir Persistence/Migrations -Project Order.Infrastructure -StartupProject Order.API
        services.AddDbContext<OrderDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });

        // 自动化数据库迁移+种子数据
        if (isDevelopment)
        {
            services.AddSeedData<OrderDbContext>();
            services.AddScoped<OrderContextSeedManager>();
            services.AddMigration<OrderDbContext, OrderContextSeed>();
        }
        else
        {
            services.AddMigration<OrderDbContext>();
        }

        return services;
    }
}
