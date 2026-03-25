namespace Order.API.Endpoints.V1.Orders;

public record CreateOrderRequest(
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    Guid PaymentMethodId,
    IReadOnlyList<OrderItemDto> OrderItems);
