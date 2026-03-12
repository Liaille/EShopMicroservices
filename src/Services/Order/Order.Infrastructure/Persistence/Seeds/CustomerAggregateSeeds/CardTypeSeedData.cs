namespace Order.Infrastructure.Persistence.Seeds.CustomerAggregateSeeds;

internal class CardTypeSeedData : ISeedData<OrderDbContext>
{
    public string AggregateName => AggregateNames.Customer;

    public int ExecuteOrder => 1;

    public async Task ExecuteAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.CardTypes.AnyAsync(cancellationToken)) return;

        await context.CardTypes.AddRangeAsync(GetPredefinedCardTypes(), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<CardType> GetPredefinedCardTypes()
    {
        yield return new CardType { Id = 1, Name = "WeChat" };
        yield return new CardType { Id = 2, Name = "AliPay" };
        yield return new CardType { Id = 3, Name = "UnionPay" };
    }
}
