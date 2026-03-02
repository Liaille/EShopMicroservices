namespace Order.Domain.Abstractions;

/// <summary>
/// 主键可能不是Id，也可能有复合主键的实体接口
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 上次修改时间
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// 上次修改者
    /// </summary>
    public string? LastModifiedBy { get; set; }
}

/// <summary>
/// 具有Id属性的单个主键的实体接口
/// </summary>
/// <typeparam name="TKey">实体的主键类型</typeparam>
public interface IEntity<TKey> : IEntity
{
    /// <summary>
    /// 此实体的唯一标识
    /// </summary>
    public TKey Id { get; }
}
