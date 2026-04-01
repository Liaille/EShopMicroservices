using BuildingBlocks.Exceptions.Handler;
using EventBus.MassTransit;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Order.API.Mappings;
using Order.API.Services;
using System.Text.Json.Serialization;
using System.Reflection;

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

        services.AddRequestMappings();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("Database")!);

        services.AddEventBus(configuration, Assembly.GetExecutingAssembly());

        services.AddHttpClient<IBasketService, BasketService>(client => client.BaseAddress = new Uri(configuration["ApiUrls:BasketAPI"]!));

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
