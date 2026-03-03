namespace Order.Domain.AggregateModels.CustomerAggregate;

public record PaymentMethodId
{
    public Guid Value { get; }

    private PaymentMethodId(Guid value) => Value = value;

    public static PaymentMethodId Create(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        if (value == Guid.Empty) throw new DomainException("PaymentMethodId cannot be empty.");
        return new PaymentMethodId(value);
    }
}
