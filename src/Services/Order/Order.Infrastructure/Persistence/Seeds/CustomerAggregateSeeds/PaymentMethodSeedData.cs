namespace Order.Infrastructure.Persistence.Seeds.CustomerAggregateSeeds;

internal class PaymentMethodSeedData : ISeedData<OrderDbContext>
{
    public string AggregateName => AggregateNames.Customer;

    public int ExecuteOrder => 3;

    public async Task ExecuteAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.PaymentMethods.AnyAsync(cancellationToken)) return;

        try
        {
            // 基础数据校验，确保卡类型存在
            int cardTypeId = 1;
            var cardType = await context.CardTypes.FirstOrDefaultAsync(ct => ct.Id == cardTypeId, cancellationToken) ?? throw new InvalidOperationException($"CardTypeId: {cardTypeId} does not exist.");
            
            // 客户数据校验，确保目标客户存在
            var targetCustomerId = CustomerId.Create(Guid.Parse("b535e7e6-aa26-432a-b260-bf0e287843d3"));
            var customerTom = await context.Customers
                .Include(c => c.PaymentMethods)
                .FirstOrDefaultAsync(c => c.Id == targetCustomerId, cancellationToken)
                ?? throw new InvalidOperationException($"CustomerId: {targetCustomerId} does not exists.");
            
            // 创建支付方式
            var paymentMethod = PaymentMethod.Create(
                cardTypeId,
                cardNumber: "62154987453221658461234", // 完整卡号，内部自动脱敏为后4位
                cardHolderName: "tom",
                expiration: DateTime.UtcNow.AddYears(3), // 3年后到期（UTC时间）
                cardType: cardType);
            
            // 客户绑定支付方式
            customerTom.AddPaymentMethod(paymentMethod);
            
            // 保存变更到数据库
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("PaymentMethod seed data insertion failed.", ex);
        }
    }
}
