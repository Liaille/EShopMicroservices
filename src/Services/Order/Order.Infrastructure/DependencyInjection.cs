using Microsoft.Extensions.Configuration;
using Order.Infrastructure.Persistence.Seeds;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(connectionString));

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
