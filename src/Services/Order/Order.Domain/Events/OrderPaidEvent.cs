namespace Order.Domain.Events;

public sealed class OrderPaidEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public string? PaymentRecordId { get; }

    public OrderPaidEvent(AggregateModels.OrderAggregate.Order order, string? paymentRecordId) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        PaymentRecordId = paymentRecordId;
    }
}
