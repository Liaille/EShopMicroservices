namespace Order.Domain.Abstractions;

/// <summary>
/// 领域事件记录
/// </summary>
public class DomainEventRecord(IDomainEvent eventData, long eventOrder)
{
    /// <summary>
    /// 领域事件数据
    /// </summary>
    public IDomainEvent EventData { get; } = eventData ?? throw new ArgumentNullException(nameof(eventData));

    /// <summary>
    /// 事件执行顺序(保证有序性)
    /// </summary>
    public long EventOrder { get; } = eventOrder;
}
