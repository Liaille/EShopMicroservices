namespace Order.Domain.Abstractions;

/// <summary>
/// 主键可能不是Id，也可能有复合主键的实体抽象基类
/// </summary>
public abstract class Entity : IEntity
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
/// 具有Id属性的单个主键定义实体抽象基类
/// </summary>
/// <typeparam name="TKey">实体的主键类型</typeparam>
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    /// <summary>
    /// 实体的唯一标识
    /// </summary>
    public virtual TKey Id { get; protected set; } = default!;

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TKey> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id!.Equals(default(TKey)) || other.Id!.Equals(default(TKey)))
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id!.GetHashCode();
    }

    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
    {
        return !(left == right);
    }
}
