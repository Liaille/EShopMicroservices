namespace Order.Domain.AggregateModels.OrderAggregate;

public record OrderName
{
    private const int DefaultLength = 5;

    public string Value { get; }

    private OrderName(string value) => Value = value;

    public static OrderName Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        
        return new OrderName(value);
    }
}
