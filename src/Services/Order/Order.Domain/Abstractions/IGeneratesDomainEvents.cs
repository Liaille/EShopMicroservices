namespace Order.Domain.Abstractions;

/// <summary>
/// 生成领域事件接口
/// </summary>
public interface IGeneratesDomainEvents
{
    /// <summary>
    /// 本地事件只读列表(仅当前进程内处理，无跨服务传播)
    /// </summary>
    IReadOnlyList<IDomainEvent> LocalEvents { get; }

    /// <summary>
    /// 分布式事件只读列表(需跨服务/进程传播)
    /// </summary>
    IReadOnlyList<IDomainEvent> DistributedEvents { get; }

    /// <summary>
    /// 清空本地事件列表
    /// </summary>
    void ClearLocalEvents();

    /// <summary>
    /// 清空分布式事件列表
    /// </summary>
    void ClearDistributedEvents();
}
