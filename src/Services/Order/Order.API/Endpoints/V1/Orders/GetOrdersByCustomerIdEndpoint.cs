using Order.Application.Queries.OrderAggregate.GetOrdersByCustomerId;

namespace Order.API.Endpoints.V1.Orders;

public class GetOrdersByCustomerIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/customers/{customerId:guid}/orders", async (
            Guid customerId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOrdersByCustomerIdQuery(customerId);
            var result = await sender.Send(query, cancellationToken);

            var response = result.Orders.Adapt<IEnumerable<OrderDto>>();

            return Results.Ok(response);
        })
        .WithName("GetOrdersByCustomerId")
        .Produces<IEnumerable<OrderDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Orders by Customer ID")
        .WithDescription("Get all orders for a customer");
    }
}
