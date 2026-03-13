namespace Order.Infrastructure.Persistence.Seeds.OrderAggregateSeeds;

internal class OrderSeedData : ISeedData<OrderDbContext>
{
    public string AggregateName => AggregateNames.Order;

    public int ExecuteOrder => 1;

    public async Task ExecuteAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        // 检验订单是否已存在，避免重复插入
        var orderId = OrderId.Create(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"));
        if (await context.Orders.AnyAsync(o => o.Id == orderId, cancellationToken)) return;

        try
        {
            // 查询依赖数据(客户、支付方式)
            var customerId = CustomerId.Create(new Guid("b535e7e6-aa26-432a-b260-bf0e287843d3"));
            var customer = await context.Customers
                .Include(c => c.PaymentMethods)
                .FirstAsync(c => c.Id == customerId, cancellationToken);
            var paymentMethodId = customer.PaymentMethods[0].Id;

            // 构建地址、订单名称等值对象
            var shippingAddress = Address.Create(
                "tom",
                "tester",
                "tom@gmail.com",
                "test address",
                "US",
                "test state",
                "100080");
            var orderName = OrderName.Create("Test Order 1");

            // 创建基础订单
            var order = Domain.AggregateModels.OrderAggregate.Order.Create(
                orderId,
                customerId,
                orderName,
                shippingAddress,
                shippingAddress,
                paymentMethodId);

            // 插入基础订单
            await context.Orders.AddAsync(order, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Order data seed created failed.", ex);
        }
    }
}
