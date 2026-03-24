namespace Order.Application.Queries.OrderAggregate.GetOrdersByOrderName;

public record GetOrdersByOrderNameQuery(string OrderName) : IQuery<GetOrdersByOrderNameResult>;

public record GetOrdersByOrderNameResult(IEnumerable<OrderDto> Orders);
