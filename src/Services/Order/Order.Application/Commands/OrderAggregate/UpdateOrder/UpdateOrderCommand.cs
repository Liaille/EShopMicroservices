namespace Order.Application.Commands.OrderAggregate.UpdateOrder;

public record UpdateOrderCommand(UpdateOrderInputDto Input) : ICommand<UpdateOrderResult>;

public record UpdateOrderResult(bool IsSuccess);
