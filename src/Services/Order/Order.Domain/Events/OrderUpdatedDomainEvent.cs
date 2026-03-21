namespace Order.Domain.Events;

/// <summary>
/// 订单信息更新事件
/// </summary>
public sealed class OrderUpdatedDomainEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderUpdatedDomainEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public OrderUpdatedDomainEvent(Guid eventId, DateTime occurredOn, AggregateModels.OrderAggregate.Order order) : base(eventId, occurredOn)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        if (occurredOn.Kind != DateTimeKind.Utc) throw new ArgumentException("The occurrence time of the event must be in UTC.", nameof(occurredOn));
    }
}
