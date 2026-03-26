using BuildingBlocks.Exceptions.Handler;
using Order.API.Mappings;

namespace Order.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddCarter();

        services.RegisterOrderMappings();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();

        app.UseExceptionHandler(options => { });

        return app;
    }
}
