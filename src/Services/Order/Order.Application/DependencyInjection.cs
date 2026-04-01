using BuildingBlocks.Behaviors;
using BuildingBlocks.Extensions;
using FluentValidation;
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
            //注册日志记录管道行为
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            // 注册参数格式验证管道行为
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            // 注册权限验证管道行为
            config.AddOpenBehavior(typeof(PermissionValidationBehavior<,>));
            // 注册业务验证管道行为
            config.AddOpenBehavior(typeof(BusinessValidationBehavior<,>));
        });
        
        // 注册所有参数格式验证器
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // 注册所有权限验证器
        services.AddPermissionValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // 注册所有业务验证器
        services.AddBusinessValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // 注册特性管理服务
        services.AddFeatureManagement();

        return services;
    }
}
