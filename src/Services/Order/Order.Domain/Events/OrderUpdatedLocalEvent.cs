namespace Order.Domain.Events;

/// <summary>
/// 订单信息更新事件
/// </summary>
public sealed class OrderUpdatedLocalEvent : DomainEvent, ILocalDomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderUpdatedLocalEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public OrderUpdatedLocalEvent(Guid eventId, DateTime occurredOn, AggregateModels.OrderAggregate.Order order) : base(eventId, occurredOn)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        if (occurredOn.Kind != DateTimeKind.Utc) throw new ArgumentException("The occurrence time of the event must be in UTC.", nameof(occurredOn));
    }
}
