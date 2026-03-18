namespace Order.Application.Dtos.Order;

/// <summary>
/// 订单项DTO
/// </summary>
/// <param name="OrderId">订单ID</param>
/// <param name="ProductId">产品Id</param>
/// <param name="Quantity">数量</param>
/// <param name="Price">价格</param>
[Serializable]
public record OrderItemDto(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal Price);
