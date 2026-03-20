namespace Order.Application.Commands.OrderAggregate.CancelOrder;

public record CancelOrderCommand(CancelOrderInputDto Input) : ICommand<CancelOrderResult>;

public record CancelOrderResult(bool IsSuccess);
