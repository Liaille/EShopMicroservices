namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketRequest(
    string UserName,
    Guid CustomerId,
    decimal TotalPrice,
    string ShippingFirstName,
    string ShippingLastName,
    string ShippingEmail,
    string ShippingCountry,
    string ShippingState,
    string ShippingAddressLine,
    string ShippingZipCode,
    string BillingFirstName,
    string BillingLastName,
    string BillingEmail,
    string BillingCountry,
    string BillingState,
    string BillingAddressLine,
    string BillingZipCode,
    Guid PaymentMethodId);

public record CheckoutBasketResponse(bool IsSuccess);

public class CheckoutBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("", async (CheckoutBasketRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = request.Adapt<CheckoutBasketCommand>();

            var result = await sender.Send(command, cancellationToken);

            var response = result.Adapt<CheckoutBasketResponse>();

            return Results.Ok(response);
        })
        .WithName("CheckoutBasket")
        .Produces<CheckoutBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Checkout Basket")
        .WithDescription("Checkout Basket");
    }
}
