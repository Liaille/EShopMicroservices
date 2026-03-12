namespace Order.Infrastructure.Persistence.Seeds.Abstractions;

/// <summary>
/// 种子数据接口
/// </summary>
/// <typeparam name="TContext"></typeparam>
public interface ISeedData<TContext> where TContext : DbContext
{
    /// <summary>
    /// 种子数据版本号(示例格式: 1.0)，用于增量更新/回滚
    /// 需要实体有相关属性，暂不启用
    /// </summary>
    //string Version { get; }

    /// <summary>
    /// 适用环境(默认Development开发环境/Staging预发布或测试环境，生产环境需显式声明Production)
    /// </summary>
    List<string> ApplyEnvs => ["Development", "Staging"];

    /// <summary>
    /// 聚合名称(适配DDD聚合拆分，用于顺序控制)
    /// </summary>
    string AggregateName { get; }

    /// <summary>
    /// 同一聚合内执行优先级(数值越小优先执行)
    /// </summary>
    int ExecuteOrder { get; }

    /// <summary>
    /// 种子数据执行方法
    /// </summary>
    Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
}
