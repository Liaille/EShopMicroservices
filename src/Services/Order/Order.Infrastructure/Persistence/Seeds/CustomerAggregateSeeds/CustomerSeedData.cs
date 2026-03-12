namespace Order.Infrastructure.Persistence.Seeds.CustomerAggregateSeeds;

internal class CustomerSeedData : ISeedData<OrderDbContext>
{
    public string AggregateName => AggregateNames.Customer;

    public int ExecuteOrder => 2;

    public async Task ExecuteAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Customers.AnyAsync(cancellationToken)) return;

        await context.Customers.AddRangeAsync(GetPredefinedCustomers(), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Customer> GetPredefinedCustomers()
    {
        yield return Customer.Create(CustomerId.Create(new Guid("b535e7e6-aa26-432a-b260-bf0e287843d3")), "tom", "tom@gmail.com");
        yield return Customer.Create(CustomerId.Create(new Guid("b238cdff-ed02-4022-b85b-511bfd79e381")), "jerry", "jerry@gmail.com");
    }
}
