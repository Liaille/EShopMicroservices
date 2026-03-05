namespace Order.Domain.AggregateModels.ProductAggregate;

/// <summary>
/// 产品
/// </summary>
public class Product : AggregateRoot<ProductId>
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; private set; } = default!;

    private Product() { }

    public static Product Create(ProductId id, string name, decimal price)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        
        Product product = new()
        {
            Id = id,
            Name = name,
            Price = price
        };

        return product;
    }
}
