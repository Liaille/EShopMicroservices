namespace Order.Domain.Abstractions;

/// <summary>
/// 聚合根的抽象基类，提供领域事件的基本实现
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    /// <summary>
    /// 本地事件列表
    /// </summary>
    private readonly List<DomainEventRecord> _localEvents = [];
    /// <summary>
    /// 分布式事件列表
    /// </summary>
    private readonly List<DomainEventRecord> _distributedEvents = [];
    /// <summary>
    /// 本地事件计数器
    /// </summary>
    private long _localEventOrderCounter;
    /// <summary>
    /// 分布式事件计数器
    /// </summary>
    private long _distributedEventOrderCounter;

    /// <summary>
    /// 本地事件只读列表(仅当前进程内处理，无跨服务传播)
    /// </summary>
    public IReadOnlyList<IDomainEvent> LocalEvents => _localEvents.Select(record => record.EventData).ToList().AsReadOnly();

    /// <summary>
    /// 分布式事件只读列表(需跨服务/进程传播)
    /// </summary>
    public IReadOnlyList<IDomainEvent> DistributedEvents => _distributedEvents.Select(record => record.EventData).ToList().AsReadOnly();

    /// <summary>
    /// 获取带顺序的本地事件记录集合
    /// </summary>
    /// <returns></returns>
    internal IReadOnlyList<DomainEventRecord> GetLocalEventRecords() => _localEvents.AsReadOnly();

    /// <summary>
    /// 获取带顺序的分布式事件记录集合
    /// </summary>
    /// <returns></returns>
    internal IReadOnlyList<DomainEventRecord> GetDistributedEventRecords() => _distributedEvents.AsReadOnly();

    /// <summary>
    /// 添加一个本地事件
    /// </summary>
    /// <param name="domainEvent"></param>
    protected virtual void AddLocalEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent, nameof(domainEvent));
        _localEvents.Add(new DomainEventRecord(domainEvent, Interlocked.Increment(ref _localEventOrderCounter)));
    }

    /// <summary>
    /// 添加一个分布式事件
    /// </summary>
    /// <param name="domainEvent"></param>
    protected virtual void AddDistributedEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent, nameof(domainEvent));
        _distributedEvents.Add(new DomainEventRecord(domainEvent, Interlocked.Increment(ref _distributedEventOrderCounter)));
    }

    /// <summary>
    /// 清空本地事件列表
    /// </summary>
    public virtual void ClearLocalEvents() => _localEvents.Clear();

    /// <summary>
    /// 清空分布式事件列表
    /// </summary>
    public virtual void ClearDistributedEvents() => _distributedEvents.Clear();
}
