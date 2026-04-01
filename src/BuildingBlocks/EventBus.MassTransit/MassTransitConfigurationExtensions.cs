using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EventBus.MassTransit;

public static class MassTransitConfigurationExtensions
{
    /// <summary>
    /// 注册 MassTransit + RabbitMQ 消息总线
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configuration">配置</param>
    /// <param name="consumerAssembly">消费者所在程序集（自动扫描注册）</param>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly? consumerAssembly = null)
    {
        // 强类型绑定消息队列配置
        var eventBusOptions = configuration
            .GetSection("EventBus")
            .Get<EventBusOptions>()!;

        services.AddMassTransit(busConfig =>
        {
            // 设置队列命名格式（kebab-case 小写+横杠命名）
            busConfig.SetKebabCaseEndpointNameFormatter();

            // 自动扫描所有IConsumer<>并注册消费者（如果传入了程序集）
            if (consumerAssembly != null)
                busConfig.AddConsumers(consumerAssembly);

            // 使用 RabbitMQ 作为消息传输载体
            busConfig.UsingRabbitMq((context, cfg) =>
            {
                // 消息队列载体连接配置
                cfg.Host(eventBusOptions.Host, host =>
                {
                    host.Username(eventBusOptions.UserName);
                    host.Password(eventBusOptions.Password);
                });

                // 生产环境必备：开启消息重试
                cfg.UseMessageRetry(retry =>
                    retry.Interval(3, TimeSpan.FromSeconds(2)));

                // 自动配置消费者端点
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
