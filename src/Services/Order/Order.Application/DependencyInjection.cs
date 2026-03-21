using BuildingBlocks.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Order.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            // 从当前执行的程序集(Order.Application.dll)中注册所有MediatR相关服务
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // 注册参数验证管道行为
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            //注册日志记录管道行为
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        return services;
    }
}
