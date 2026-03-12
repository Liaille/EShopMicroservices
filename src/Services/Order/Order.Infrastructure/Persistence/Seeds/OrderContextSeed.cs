namespace Order.Infrastructure.Persistence.Seeds;

public class OrderContextSeed(OrderContextSeedManager seedManager) : IDbSeeder<OrderDbContext>
{
    public async Task SeedAsync(OrderDbContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        await seedManager.ExecuteAllAsync(context, cancellationToken);
    }
}
