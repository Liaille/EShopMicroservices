namespace Order.Domain.Abstractions;

/// <summary>
/// 聚合根的抽象基类，提供领域事件的基本实现
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    /// <summary>
    /// 领域事件记录列表 (包含顺序)
    /// </summary>
    private readonly List<DomainEventRecord> _domainEvents = [];

    /// <summary>
    /// 事件顺序计数器 (保证事件处理的先后顺序)
    /// </summary>
    private long _eventOrderCounter;

    /// <summary>
    /// 领域事件只读列表
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.Select(record => record.EventData).ToList().AsReadOnly();

    /// <summary>
    /// 获取带执行顺序的事件记录只读列表 (供基础设施层分发时排序)
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<DomainEventRecord> GetDomainEventRecords() => _domainEvents.AsReadOnly();

    /// <summary>
    /// 添加一个本地事件
    /// </summary>
    /// <param name="domainEvent"></param>
    protected virtual void AddDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent, nameof(domainEvent));
        _domainEvents.Add(new DomainEventRecord(domainEvent, Interlocked.Increment(ref _eventOrderCounter)));
    }

    /// <summary>
    /// 清空本地事件列表
    /// </summary>
    public virtual void ClearDomainEvents() => _domainEvents.Clear();

}
