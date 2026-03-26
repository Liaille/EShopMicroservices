using Order.Application.Queries.OrderAggregate.GetOrdersByOrderName;

namespace Order.API.Endpoints.V1.Orders;

public class GetOrdersByOrderNameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // /api/v1/orders/search/by-name?orderName=abc
        app.MapGet("/api/v1/orders/search/by-name", async (
            [FromQuery] string orderName, // 直接从 URL 查询参数获取
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOrdersByOrderNameQuery(orderName);

            var result = await sender.Send(query, cancellationToken);

            var response = result.Orders.Adapt<IEnumerable<OrderDto>>();

            return Results.Ok(response);
        })
        .WithName("GetOrdersByOrderName")
        .Produces<IEnumerable<OrderDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Search orders by order name")
        .WithDescription("Query orders by matching order name (full or partial)");
    }
}
