namespace Order.Domain.AggregateModels.CustomerAggregate;

public record CustomerId
{
    public Guid Value { get; }

    private CustomerId(Guid value) => Value = value;

    public static CustomerId Create(Guid value)
    {
        if (value == Guid.Empty) throw new DomainException("CustomerId cannot be empty.");

        return new CustomerId(value);
    }
}
