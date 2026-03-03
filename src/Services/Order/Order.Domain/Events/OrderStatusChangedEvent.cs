namespace Order.Domain.Events;

public sealed class OrderStatusChangedEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderStatus NewStatus { get; }

    public OrderStatusChangedEvent(AggregateModels.OrderAggregate.Order order, OrderStatus newStatus) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        NewStatus = newStatus;
    }
}
