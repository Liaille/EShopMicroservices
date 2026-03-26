namespace Order.Application.Commands.OrderAggregate.CreateOrder;

public class CreateOrderBusinessValidator(IOrderDbContext dbContext) : IBusinessValidator<CreateOrderCommand>
{
    public async Task ValidateAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        // 1. 校验支付方式是否存在 & 归属当前客户
        bool isValidPayment = await dbContext.PaymentMethods
            .AnyAsync(p =>
                p.Id == PaymentMethodId.Create(command.Input.PaymentMethodId) &&
                EF.Property<CustomerId>(p, "CustomerId") == CustomerId.Create(command.Input.CustomerId),
                cancellationToken);

        if (!isValidPayment)
            throw new BadRequestException("The payment method does not exist or is not associated with the current user.");

        // 2. 获取所有商品ID（强类型）
        var productIds = command.Input.OrderItems
            .Select(oi => ProductId.Create(oi.ProductId))
            .ToList();

        // 3. 查询数据库中存在的商品ID
        var existingProductIds = await dbContext.Products
            .Where(p => productIds.Contains(EF.Property<ProductId>(p, "Id")))
            .Select(p => EF.Property<ProductId>(p, "Id"))
            .ToListAsync(cancellationToken);

        // 4. 找出不存在的商品
        var missingIds = productIds.Except(existingProductIds).ToList();

        if (missingIds.Count != 0)
            throw new BadRequestException($"The following products do not exist: {string.Join(", ", missingIds.Select(x => x.Value))}");
    }
}
