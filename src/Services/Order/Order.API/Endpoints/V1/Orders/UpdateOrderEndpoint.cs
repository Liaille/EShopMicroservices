using Order.Application.Commands.OrderAggregate.UpdateOrder;

namespace Order.API.Endpoints.V1.Orders;

public class UpdateOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/orders/{id:guid}", async (
            Guid id,          // 路由校验ID
            UpdateOrderRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            // 路由ID 与 请求体ID 保持一致（安全校验）
            if (id != request.OrderId)
                return Results.BadRequest("Order ID mismatch");

            var command = request.Adapt<UpdateOrderCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<UpdateOrderResponse>();

            return Results.Ok(response);
        })
        .WithName("UpdateOrder")
        .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Order")
        .WithDescription("Update order basic information, including address and payment method");
    }
}
