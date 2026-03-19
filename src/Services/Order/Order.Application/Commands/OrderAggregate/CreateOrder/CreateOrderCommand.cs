namespace Order.Application.Commands.OrderAggregate.CreateOrder;

public record CreateOrderCommand(CreateOrderInputDto Input) : ICommand<CreateOrderResult>;

public record CreateOrderResult(Guid Id);
