using BuildingBlocks.Validations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Extensions;

public static class ValidationExtension
{
    // 注册权限校验器
    public static IServiceCollection AddPermissionValidatorsFromAssembly(
        this IServiceCollection services, Assembly assembly)
    {
        services.RegisterGenericImplementations(assembly, typeof(IPermissionValidator<>));
        return services;
    }

    // 注册业务校验器
    public static IServiceCollection AddBusinessValidatorsFromAssembly(
        this IServiceCollection services, Assembly assembly)
    {
        services.RegisterGenericImplementations(assembly, typeof(IBusinessValidator<>));
        return services;
    }
}
