namespace Order.Domain.Events;

public sealed class OrderUpdatedEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderUpdatedEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public OrderUpdatedEvent(Guid eventId, DateTime occurredOn, AggregateModels.OrderAggregate.Order order) : base(eventId, occurredOn)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        if (occurredOn.Kind != DateTimeKind.Utc) throw new ArgumentException("The occurrence time of the event must be in UTC.", nameof(occurredOn));
    }
}
