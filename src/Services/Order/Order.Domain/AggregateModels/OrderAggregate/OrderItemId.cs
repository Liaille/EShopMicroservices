namespace Order.Domain.AggregateModels.OrderAggregate;

public record OrderItemId
{
    public Guid Value { get; }

    private OrderItemId(Guid value) => Value = value;

    public static OrderItemId Create(Guid value)
    {
        if (value == Guid.Empty) throw new DomainException("OrderItemId cannot be empty.");
        return new OrderItemId(value);
    }
}
