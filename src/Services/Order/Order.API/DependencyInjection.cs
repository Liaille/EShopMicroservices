using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Order.API.Mappings;
using System.Text.Json.Serialization;

namespace Order.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JsonOptions>(options =>
        {
            // 支持枚举字符串/数字互转
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            // 允许数字转枚举
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        });

        services.AddCarter();

        services.RegisterOrderMappings();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("Database")!);

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();

        app.UseExceptionHandler(options => { });

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}
