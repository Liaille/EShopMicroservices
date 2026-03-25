namespace Order.API.Endpoints.V1.Orders;

public record CancelOrderRequest(
    Guid OrderId,
    Guid CustomerId,
    string? CancelReason);
