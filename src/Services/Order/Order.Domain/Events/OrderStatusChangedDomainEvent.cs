namespace Order.Domain.Events;

/// <summary>
/// 订单状态变更事件
/// </summary>
public sealed class OrderStatusChangedDomainEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderStatus NewStatus { get; }

    public OrderStatusChangedDomainEvent(AggregateModels.OrderAggregate.Order order, OrderStatus newStatus) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        NewStatus = newStatus;
    }
}
