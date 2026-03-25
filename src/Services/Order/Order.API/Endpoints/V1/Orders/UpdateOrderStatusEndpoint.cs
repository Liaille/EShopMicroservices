using Order.Application.Commands.OrderAggregate.UpdateOrderStatus;

namespace Order.API.Endpoints.V1.Orders;

public class UpdateOrderStatusEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/v1/orders/{id:guid}/status", async (
            Guid id,
            UpdateOrderStatusRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (id != request.OrderId)
                return Results.BadRequest("Order ID mismatch");

            var command = request.Adapt<UpdateOrderStatusCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<UpdateOrderStatusResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateOrderStatus")
        .Produces<UpdateOrderStatusResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Order Status")
        .WithDescription("Update order status, optionally with payment record ID");
    }
}
