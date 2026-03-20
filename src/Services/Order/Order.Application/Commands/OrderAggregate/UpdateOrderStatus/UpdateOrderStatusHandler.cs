using Order.Application.Interfaces.Repositories;

namespace Order.Application.Commands.OrderAggregate.UpdateOrderStatus;

public class UpdateOrderStatusHandler(IOrderDbContext dbContext) : ICommandHandler<UpdateOrderStatusCommand, UpdateOrderStatusResult>
{
    public async Task<UpdateOrderStatusResult> Handle(UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Create(command.Input.OrderId);
        var order = await dbContext.Orders.FindAsync([orderId], cancellationToken) ?? throw new OrderNotFoundException(orderId.Value);

        UpdateOrderStatus(order, command.Input);

        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateOrderStatusResult(true);
    }

    private static void UpdateOrderStatus(Domain.AggregateModels.OrderAggregate.Order order, UpdateOrderStatusInputDto inputDto)
    {
        switch (inputDto.TargetStatus)
        {
            case OrderStatus.AwaitingValidation:
                order.MarkAsAwaitingValidation();
                break;
            case OrderStatus.StockConfirmed:
                order.MarkAsStockConfirmed();
                break;
            case OrderStatus.Paid:
                order.MarkAsPaid(inputDto.PaymentRecordId);
                break;
            case OrderStatus.Shipped:
                order.MarkAsShipped();
                break;
            default:
                throw new InvalidOperationException($"Unsupported state flow: {inputDto.TargetStatus}");
        }
    }
}
