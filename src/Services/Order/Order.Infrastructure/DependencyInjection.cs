using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Infrastructure.Persistence.DbContexts;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
