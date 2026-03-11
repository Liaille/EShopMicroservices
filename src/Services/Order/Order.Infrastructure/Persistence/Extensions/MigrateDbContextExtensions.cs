using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Order.Infrastructure.Persistence.Extensions;

/// <summary>
/// 自动执行EF Core数据库迁移+种子数据的扩展方法类
/// </summary>
public static class MigrateDbContextExtensions
{
    /// <summary>
    /// .NET可观测性体系中用于标识和跟踪数据库迁移活动的ActivitySource名称。
    /// </summary>
    private static readonly string s_activitySourceName = "DbMigrations";

    /// <summary>
    /// .NET中生成追踪活动的ActivitySource实例，用于在数据库迁移过程中创建和管理活动，以便进行性能监视和诊断。
    /// </summary>
    private static readonly ActivitySource s_activitySource = new(s_activitySourceName);

    /// <summary>
    /// 向 ASP.NET Core 服务容器注册指定 EF Core 数据库上下文的自动迁移后台服务（无种子数据初始化），并集成 OpenTelemetry 分布式追踪。
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMigration<TContext>(this IServiceCollection services) 
        where TContext : DbContext
    => services.AddMigration<TContext>((_, _) => Task.CompletedTask);
    
    /// <summary>
    /// 向 ASP.NET Core 服务容器注册指定 EF Core 数据库上下文的自动迁移后台服务，并集成 OpenTelemetry 分布式追踪。
    /// </summary>
    /// <typeparam name="TContext">待执行迁移的 EF Core 数据库上下文类型，必须继承自 <see cref="DbContext"/>。</typeparam>
    /// <param name="services">ASP.NET Core 服务集合（<see cref="IServiceCollection"/>），用于注册后台服务和 OpenTelemetry 组件，不能为空。</param>
    /// <param name="seeder">种子数据初始化委托，接收数据库上下文和作用域内服务提供器，返回异步任务；
    /// 若无需初始化种子数据，可传入空委托（如 (_, _) => Task.CompletedTask）。</param>
    /// <returns>添加了迁移后台服务的 <see cref="IServiceCollection"/> 实例，支持链式调用。</returns>
    public static IServiceCollection AddMigration<TContext>(
        this IServiceCollection services, 
        Func<TContext, IServiceProvider, Task> seeder) 
        where TContext : DbContext
    {
        services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(s_activitySourceName));

        return services.AddHostedService(sp => new MigrationHostedService<TContext>(sp, seeder));
    }

    /// <summary>
    /// 向 ASP.NET Core 服务容器注册指定 EF Core 数据库上下文的自动迁移后台服务，
    /// 并集成基于 <see cref="IDbSeeder{TContext}"/> 接口的种子数据初始化逻辑，同时启用 OpenTelemetry 分布式追踪。
    /// </summary>
    /// <typeparam name="TContext">待执行迁移的 EF Core 数据库上下文类型，必须继承自 <see cref="DbContext"/>。</typeparam>
    /// <typeparam name="TDbSeeder">实现 <see cref="IDbSeeder{TContext}"/> 接口的种子数据初始化类，
    /// 必须为引用类型（class），且与 <typeparamref name="TContext"/> 上下文类型匹配。</typeparam>
    /// <param name="services">ASP.NET Core 服务集合（<see cref="IServiceCollection"/>），用于注册种子数据服务和迁移后台服务，不能为空。</param>
    /// <returns>添加了迁移服务和种子数据服务的 <see cref="IServiceCollection"/> 实例，支持链式调用。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="services"/> 为 null 时抛出。</exception>
    /// <exception cref="InvalidOperationException">当 <typeparamref name="TDbSeeder"/> 未正确实现 <see cref="IDbSeeder{TContext}"/> 接口时抛出（运行时解析服务时）。</exception>
    public static IServiceCollection AddMigration<TContext, TDbSeeder>(this IServiceCollection services)
        where TContext : DbContext
        where TDbSeeder : class, IDbSeeder<TContext>
    {
        services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();

        return services.AddMigration<TContext>((context, serviceProvider) => serviceProvider.GetRequiredService<IDbSeeder<TContext>>().SeedAsync(context));
    }

    /// <summary>
    /// 为指定的 EF Core 数据库上下文执行带容错策略的数据库迁移 + 种子数据初始化，并集成分布式追踪与结构化日志。
    /// </summary>
    /// <typeparam name="TContext">待迁移的 EF Core 数据库上下文类型，必须继承自 <see cref="DbContext"/>。</typeparam>
    /// <param name="services">应用程序根服务提供器，用于创建独立作用域并解析上下文/日志等依赖，不能为空。</param>
    /// <param name="seeder">种子数据初始化委托，接收数据库上下文和作用域内服务提供器，返回异步任务；
    /// 若无需初始化种子数据，可传入空委托（如 (_, _) => Task.CompletedTask）。</param>
    /// <returns>表示异步迁移+种子数据操作的 <see cref="Task"/>。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="services"/> 为 null，或解析 <typeparamref name="TContext"/>/<see cref="ILogger{TContext}"/> 失败时抛出。</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.Migrations.MigrationException">数据库迁移（<see cref="DbContext.Database.MigrateAsync"/>）执行失败时抛出。</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.Storage.RetryLimitExceededException">EF Core 执行策略重试次数耗尽仍失败时抛出。</exception>
    /// <exception cref="Exception">种子数据初始化委托 <paramref name="seeder"/> 执行过程中抛出的任意异常（会原样抛出并记录到追踪/日志）。</exception>
    private static async Task MigrateDbContextAsync<TContext>(this IServiceProvider services,
        Func<TContext, IServiceProvider, Task> seeder) where TContext : DbContext
    {
        using var scope = services.CreateScope();
        var scopeServices = scope.ServiceProvider;
        var logger = scopeServices.GetRequiredService<ILogger<TContext>>();
        var context = scopeServices.GetRequiredService<TContext>();

        using var activity = s_activitySource.StartActivity($"Migration operation {typeof(TContext).Name}");

        try
        {
            if(logger.IsEnabled(LogLevel.Information)) 
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            var strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(() => InvokeSeeder(seeder, context, scopeServices));
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(ex, "An error occurred while migrating the database associated with context {DbContextName}", typeof(TContext).Name);

            activity?.SetExceptionTags(ex);

            throw;
        }
    }

    /// <summary>
    /// 执行 EF Core 数据库迁移并调用种子数据初始化逻辑，同时生成符合 OpenTelemetry 规范的追踪活动。
    /// </summary>
    /// <typeparam name="TContext">EF Core 数据库上下文类型，必须继承自 <see cref="DbContext"/>。</typeparam>
    /// <param name="seeder">种子数据初始化委托，接收数据库上下文和服务提供器，返回异步任务；
    /// 委托逻辑通常包含基础数据（如卡类型、订单状态）的插入操作。</param>
    /// <param name="context">待执行迁移的 EF Core 数据库上下文实例，不能为空。</param>
    /// <param name="services">应用程序服务提供器，用于解析种子数据逻辑所需的依赖（如日志、配置），不能为空。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="seeder"/>、<paramref name="context"/> 或 <paramref name="services"/> 为 null 时抛出（由调用方或 EF Core 内部触发）。</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.Migrations.MigrationException">执行 <see cref="DbContext.Database.MigrateAsync"/> 时迁移失败抛出。</exception>
    /// <exception cref="Exception">种子数据初始化委托 <paramref name="seeder"/> 执行过程中抛出的任意异常（会原样抛出并记录到追踪活动）。</exception>
    private static async Task InvokeSeeder<TContext>(
        Func<TContext, IServiceProvider, Task> seeder,
        TContext context,
        IServiceProvider services)
        where TContext : DbContext
    {
        using var activity = s_activitySource.StartActivity($"Seeding operation {typeof(TContext).Name}");

        try
        {
            await context.Database.MigrateAsync();
            await seeder(context, services);
        }
        catch (Exception ex)
        {
            activity?.SetExceptionTags(ex);

            throw;
        }
    }

    /// <summary>
    /// ASP.NET Core 后台服务（<see cref="BackgroundService"/>），用于应用启动时自动触发指定 EF Core 数据库上下文的迁移 + 种子数据初始化。
    /// </summary>
    /// <typeparam name="TContext">待执行迁移的 EF Core 数据库上下文类型，必须继承自 <see cref="DbContext"/>。</typeparam>
    /// <param name="serviceProvider">应用程序根服务提供器，用于解析依赖并调用 <see cref="MigrateDbContextAsync{TContext}"/> 扩展方法，不能为空。</param>
    /// <param name="seeder">种子数据初始化委托，接收数据库上下文和服务提供器，返回异步任务；若无需种子数据，可传入空委托。</param>
    private class MigrationHostedService<TContext>(
        IServiceProvider serviceProvider,
        Func<TContext, IServiceProvider, Task> seeder) : BackgroundService where TContext : DbContext
    {
        /// <summary>
        /// 服务启动方法（重写），应用启动时自动调用，触发数据库迁移 + 种子数据初始化。
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于中断迁移操作（此处未处理，迁移操作会执行至完成）。</param>
        /// <returns>表示迁移+种子数据操作的 <see cref="Task"/>，迁移完成则任务完成，迁移失败则任务抛出异常。</returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return serviceProvider.MigrateDbContextAsync(seeder);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}

/// <summary>
/// 定义 EF Core 数据库上下文的种子数据初始化接口，规范基础数据/测试数据的插入逻辑。
/// </summary>
/// <typeparam name="TContext">目标 EF Core 数据库上下文类型，必须继承自 <see cref="DbContext"/>；
/// 使用 in 修饰符表示该类型参数为逆变，仅作为输入参数使用（符合种子数据操作的只读上下文特性）。</typeparam>
public interface IDbSeeder<in TContext> where TContext : DbContext
{
    /// <summary>
    /// 异步执行指定数据库上下文的种子数据初始化操作。
    /// </summary>
    /// <param name="context">待初始化种子数据的 EF Core 数据库上下文实例，不能为空。</param>
    /// <returns>表示种子数据插入操作的 <see cref="Task"/>，操作完成则任务完成，插入失败则任务抛出异常。</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="context"/> 为 null 时抛出。</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">插入数据时违反数据库约束（如主键重复、外键关联失败）时抛出。</exception>
    Task SeedAsync(TContext context);
}
