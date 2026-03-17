namespace Order.Domain.Events;

/// <summary>
/// 订单状态变更事件
/// </summary>
public sealed class OrderStatusChangedLocalEvent : DomainEvent, ILocalDomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderStatus NewStatus { get; }

    public OrderStatusChangedLocalEvent(AggregateModels.OrderAggregate.Order order, OrderStatus newStatus) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        NewStatus = newStatus;
    }
}
