namespace Order.Domain.Abstractions;

/// <summary>
/// 主键可能不是Id，也可能有复合主键的聚合根接口
/// </summary>
public interface IAggregateRoot : IEntity, IGeneratesDomainEvents
{
}

/// <summary>
/// 具有Id属性的单个主键的聚合根接口
/// </summary>
/// <typeparam name="TKey">实体的主键类型</typeparam>
public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
{
}
