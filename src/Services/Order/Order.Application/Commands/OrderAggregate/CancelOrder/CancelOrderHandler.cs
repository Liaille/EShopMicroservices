using Order.Application.Interfaces.Repositories;

namespace Order.Application.Commands.OrderAggregate.CancelOrder;

public class CancelOrderHandler(IOrderDbContext dbContext) : ICommandHandler<CancelOrderCommand, CancelOrderResult>
{
    public async Task<CancelOrderResult> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Create(command.Input.OrderId);
        var order = await dbContext.Orders.FindAsync([orderId], cancellationToken) ?? throw new OrderNotFoundException(orderId.Value);

        if (order.CustomerId != CustomerId.Create(command.Input.CustomerId)) throw new InvalidOperationException("Customer ID inconsistent, unauthorized operation.");

        order.Cancel();
        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CancelOrderResult(true);
    }
}
