namespace Order.Infrastructure.Persistence.Seeds.OrderAggregateSeeds;

internal class OrderItemSeedData : ISeedData<OrderDbContext>
{
    public string AggregateName => AggregateNames.Order;

    public int ExecuteOrder => 2;

    public async Task ExecuteAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        // 无导航属性时，直接通过EF Core的外键约定查询
        var orderId = OrderId.Create(Guid.Parse("5334c996-8457-4cf0-815c-ed2b77c4ff61"));
        if (await context.Orders.AnyAsync(oi => EF.Property<Guid>(oi, "OrderId") == orderId.Value, cancellationToken)) return;

        try
        {
            // 查询已有订单
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstAsync(o => o.Id == orderId, cancellationToken);

            // 查询商品数据
            var productId = ProductId.Create(Guid.Parse("81de7a19-6f4a-490f-802c-c5618ebdd4f9"));
            var product = await context.Products.FirstAsync(p => p.Id == productId, cancellationToken);

            // 为订单添加订单项
            order.AddOrderItem(product.Id, 1, product.Price);

            // 保存更改
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Order item seed data addition failed.", ex);
        }

    }
}
