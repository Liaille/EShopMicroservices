using Order.Application.Queries.GetOrders;

namespace Order.API.Endpoints.V1.Orders;

public class GetOrdersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/orders", async (
            [AsParameters] PaginationRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOrdersQuery(request);

            var result = await sender.Send(query, cancellationToken);

            var response = result.Orders.Adapt<PaginatedResult<OrderDto>>();

            return Results.Ok(response);
        })
        .WithName("GetOrders")
        .Produces<PaginatedResult<OrderDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Orders with Pagination")
        .WithDescription("Get a paginated list of orders");
    }
}
