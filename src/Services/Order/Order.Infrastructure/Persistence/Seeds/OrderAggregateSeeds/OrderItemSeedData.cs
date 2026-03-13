namespace Order.Infrastructure.Persistence.Seeds.OrderAggregateSeeds;

internal class OrderItemSeedData : ISeedData<OrderDbContext>
{
    public string AggregateName => AggregateNames.Order;

    public int ExecuteOrder => 2;

    public async Task ExecuteAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // 创建值对象实例
            var targetOrderId = OrderId.Create(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"));
            var targetProductId = ProductId.Create(new Guid("81de7a19-6f4a-490f-802c-c5618ebdd4f9"));

            // 查询订单项是否存在
            if (await context.OrderItems
                .AnyAsync(oi => oi.OrderId == targetOrderId
                              && oi.ProductId == targetProductId, cancellationToken))
                return;

            // 查询订单
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstAsync(o => o.Id == targetOrderId, cancellationToken);

            // 查询商品
            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == targetProductId, cancellationToken)
                ?? throw new InvalidOperationException($"ProductId: {targetProductId.Value} does not exist.");

            // 检查订单项重复
            if (!order.OrderItems.Any(oi => oi.ProductId == targetProductId))
                order.AddOrderItem(product.Id, 1, product.Price);

            // 保存更改
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Order item seed data addition failed: {ex.Message}", ex);
        }
    }
}
