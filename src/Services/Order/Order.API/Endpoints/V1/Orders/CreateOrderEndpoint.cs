using Order.Application.Commands.OrderAggregate.CreateOrder;

namespace Order.API.Endpoints.V1.Orders;

public class CreateOrderEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/orders", async (
            CreateOrderRequest request, 
            ISender sender, 
            CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<CreateOrderCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<CreateOrderResponse>();

            return Results.Created($"/api/v1/orders/{result.Id}", response);
        })
        .WithName("CreateOrder")
        .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Order")
        .WithDescription("Create a new order with order items, shipping/billing address and payment method info.");
    }
}
