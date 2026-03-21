namespace Order.Domain.Abstractions;

/// <summary>
/// 领域事件接口 (仅描述领域事实，不区分本地/分布式)
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
