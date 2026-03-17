namespace Order.Domain.Abstractions;

/// <summary>
/// 领域事件接口
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// 事件发生的时间
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// 事件类型
    /// </summary>
    public string EventType { get; }
}

/// <summary>
/// 本地事件标记接口(仅当前进程内处理)
/// </summary>
public interface ILocalDomainEvent : IDomainEvent { }

/// <summary>
/// 分布式事件标记接口(需跨服务传播)
/// </summary>
public interface IDistributedDomainEvent : IDomainEvent { }
