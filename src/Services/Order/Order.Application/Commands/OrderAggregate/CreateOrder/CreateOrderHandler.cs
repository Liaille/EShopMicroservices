namespace Order.Application.Commands.OrderAggregate.CreateOrder;

public class CreateOrderHandler(IOrderDbContext dbContext) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = CreateOrder(command.Input);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id.Value);
    }

    private static Domain.AggregateModels.OrderAggregate.Order CreateOrder(CreateOrderInputDto orderDto)
    {
        var shippingAddress = Address.Create(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.Email ?? string.Empty, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var billingAddress = Address.Create(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.Email ?? string.Empty, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);

        var order = Domain.AggregateModels.OrderAggregate.Order.Create(
            id: OrderId.Create(Guid.NewGuid()),
            customerId: CustomerId.Create(orderDto.CustomerId),
            orderName: OrderName.Create(orderDto.OrderName),
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            paymentMethodId: PaymentMethodId.Create(orderDto.PaymentMethodId));

        foreach (var orderItemDto in orderDto.OrderItems)
        {
            order.AddOrderItem(ProductId.Create(orderItemDto.ProductId), orderItemDto.Quantity, orderItemDto.Price);
        }

        return order;
    }
}
