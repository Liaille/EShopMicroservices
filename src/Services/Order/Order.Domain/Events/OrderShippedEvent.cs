namespace Order.Domain.Events;

public sealed class OrderShippedEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderShippedEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }
}
