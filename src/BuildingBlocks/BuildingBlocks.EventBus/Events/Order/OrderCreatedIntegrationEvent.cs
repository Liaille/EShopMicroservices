namespace BuildingBlocks.EventBus.Events.Order;

public record OrderCreatedIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// 订单总价
    /// </summary>
    public decimal TotalPrice { get; init; }
}
