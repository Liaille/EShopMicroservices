using Order.Application.Commands.OrderAggregate.CancelOrder;

namespace Order.API.Endpoints.V1.Orders;

public class CancelOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/orders/cancel", async (
            CancelOrderRequest request, 
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<CancelOrderCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<CancelOrderResponse>();

            return Results.Ok(response);
        })
        .WithName("CancelOrder")
        .Produces<CancelOrderResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Cancel Order")
        .WithDescription("Cancel an existing order, requires OrderId, CustomerId and optional CancelReason");
    }
}
