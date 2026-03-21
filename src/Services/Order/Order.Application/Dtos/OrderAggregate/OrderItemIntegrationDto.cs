namespace Order.Application.Dtos.OrderAggregate;

/// <summary>
/// 订单项集成DTO
/// </summary>
/// <param name="ProductId">商品ID</param>
/// <param name="Quantity">购买数量</param>
/// <param name="Price">商品单价</param>
public record OrderItemIntegrationDto(
    Guid ProductId,
    int Quantity,
    decimal Price)
{
    /// <summary>
    /// 从领域层 <see cref="OrderItem"/> 转换为 <see cref="OrderItemIntegrationDto"/>
    /// </summary>
    /// <param name="orderItem">领域层订单项对象</param>
    /// <returns><see cref="OrderItemIntegrationDto"/></returns>
    public static OrderItemIntegrationDto FromDomain(OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(orderItem, nameof(orderItem));

        return new OrderItemIntegrationDto(
            orderItem.ProductId.Value,
            orderItem.Quantity,
            orderItem.Price);
    }

    /// <summary>
    /// 仅校验跨服务传输必需字段
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        return ProductId != Guid.Empty 
            && Quantity > 0 
            && Price >=0;
    }
};
