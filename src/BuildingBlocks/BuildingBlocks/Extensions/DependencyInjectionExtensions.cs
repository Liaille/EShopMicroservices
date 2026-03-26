using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Extensions;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// 自动批量注册 实现了 泛型接口 的类（原生DI，无第三方依赖）
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly">要扫描的程序集</param>
    /// <param name="openGenericInterface">开放泛型接口，如 IPermissionValidation<></param>
    /// <param name="lifetime">生命周期</param>
    public static void RegisterGenericImplementations(
        this IServiceCollection services,
        Assembly assembly,
        Type openGenericInterface,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // 找到所有实现类
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false })
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface))
            .ToList();

        foreach (var type in types)
        {
            var interfaceType = type.GetInterfaces().First(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface);

            services.Add(ServiceDescriptor.Describe(interfaceType, type, lifetime));
        }
    }
}
