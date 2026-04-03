namespace Order.Infrastructure.Persistence.Seeds.ProductAggregateSeeds;

internal class ProductSeedData : ISeedData<OrderDbContext>
{
    public string AggregateName => AggregateNames.Product;

    public int ExecuteOrder => 1;

    public async Task ExecuteAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Products.AnyAsync(cancellationToken)) return;

        await context.Products.AddRangeAsync(GetPredefinedProducts(), cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Product> GetPredefinedProducts()
    {
        yield return Product.Create(ProductId.Create(new Guid("81de7a19-6f4a-490f-802c-c5618ebdd4f9")), "IPhone 14", 799.99m);
        yield return Product.Create(ProductId.Create(new Guid("bb68a0d6-a11b-4b5d-aa7a-2913906372fd")), "Samsung Galaxy S22", 699.99m);
        yield return Product.Create(ProductId.Create(new Guid("8f6dff7a-116b-49c3-94d3-4896844a5df8")), "Google Pixel 6", 599.99m);
        yield return Product.Create(ProductId.Create(new Guid("2169b902-25f3-4a4a-a63e-8504a6f7bc67")), "OnePlus 9", 729.99m);
    }
}
