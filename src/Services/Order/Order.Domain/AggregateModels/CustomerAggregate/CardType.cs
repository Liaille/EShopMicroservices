namespace Order.Domain.AggregateModels.CustomerAggregate;

/// <summary>
/// 支付渠道类型
/// </summary>
public sealed class CardType
{
    public int Id { get; init; }

    public required string Name { get; init; }
}
