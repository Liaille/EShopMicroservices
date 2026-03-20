namespace Order.Application.Commands.OrderAggregate.UpdateOrderStatus;

public record UpdateOrderStatusCommand(UpdateOrderStatusInputDto Input) : ICommand<UpdateOrderStatusResult>;

public record UpdateOrderStatusResult(bool IsSuccess);
