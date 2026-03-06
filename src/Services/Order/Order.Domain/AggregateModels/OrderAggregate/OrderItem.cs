namespace Order.Domain.AggregateModels.OrderAggregate;

/// <summary>
/// 订单项
/// </summary>
public class OrderItem : Entity<OrderItemId> 
{
    /// <summary>
    /// 产品Id
    /// </summary>
    public ProductId ProductId { get; private set; } = default!;

    /// <summary>
    /// 数量
    /// </summary>
    public int Quantity { get; private set; } = default!;

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; private set; } = default!;

    internal OrderItem(ProductId productId, int quantity, decimal price)
    {
        Id = OrderItemId.Create(Guid.NewGuid());
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}
