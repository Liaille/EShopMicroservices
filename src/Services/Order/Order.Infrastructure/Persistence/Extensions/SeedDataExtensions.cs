using System.Reflection;

namespace Order.Infrastructure.Persistence.Extensions;

public static class SeedDataExtensions
{
    /// <summary>
    /// 自动扫描注册实现了ISeedData接口的类到DI容器中，以便在应用程序启动时执行数据种子逻辑。
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSeedData<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        var assembly = Assembly.GetAssembly(typeof(TContext));
        var seedType = typeof(ISeedData<TContext>);

        // 扫描所有实现ISeedData<TContext>的非抽象类
        var seedClasses = assembly?.GetTypes()
            .Where(t => seedType.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
            .ToList();

        // 将这些类自动注册为Scoped到DI容器中
        foreach (var seedClass in seedClasses ?? Enumerable.Empty<Type>())
        {
            services.AddScoped(seedType, seedClass);
        }

        return services;
    }
}
