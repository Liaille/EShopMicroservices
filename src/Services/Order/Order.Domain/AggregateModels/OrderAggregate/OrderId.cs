namespace Order.Domain.AggregateModels.OrderAggregate;

public record OrderId
{
    public Guid Value { get; }

    private OrderId(Guid value) => Value = value;

    public static OrderId Create(Guid value)
    {
        if (value == Guid.Empty) throw new DomainException("OrderId cannot be empty.");
        return new OrderId(value);
    }
}
