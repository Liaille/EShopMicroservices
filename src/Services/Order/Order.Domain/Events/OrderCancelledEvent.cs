namespace Order.Domain.Events;

public sealed class OrderCancelledEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderCancelledEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }
}
