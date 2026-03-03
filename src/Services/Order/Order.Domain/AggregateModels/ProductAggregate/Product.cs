namespace Order.Domain.AggregateModels.ProductAggregate;

/// <summary>
/// 产品
/// </summary>
public class Product : AggregateRoot<Guid>
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; private set; } = default!;
}
