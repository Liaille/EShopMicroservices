namespace Order.Application.Queries.OrderAggregate.GetOrdersByCustomerId;

public record GetOrdersByCustomerIdQuery(Guid CustomerId) : IQuery<GetOrdersByCustomerIdResult>;

public record GetOrdersByCustomerIdResult(IEnumerable<OrderDto> Orders);
