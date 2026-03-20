using Order.Application.Interfaces.Repositories;

namespace Order.Application.Commands.OrderAggregate.UpdateOrder;

public class UpdateOrderHandler(IOrderDbContext dbContext) : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
{
    public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Create(command.Input.OrderId);
        var order = await dbContext.Orders.FindAsync([orderId], cancellationToken) ?? throw new OrderNotFoundException(command.Input.OrderId);

        UpdateOrderWithNewValues(order, command.Input);

        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateOrderResult(true);
    }

    private static void UpdateOrderWithNewValues(Domain.AggregateModels.OrderAggregate.Order order, UpdateOrderInputDto orderDto)
    {
        var shippingAddress = Address.Create(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.Email ?? string.Empty, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var billingAddress = Address.Create(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.Email ?? string.Empty, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);

        order.Update(
            orderName: OrderName.Create(orderDto.OrderName),
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            paymentMethodId: PaymentMethodId.Create(orderDto.PaymentMethodId));
    }
}
