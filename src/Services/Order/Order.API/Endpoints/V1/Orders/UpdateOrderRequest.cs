namespace Order.API.Endpoints.V1.Orders;

public record UpdateOrderRequest(
    Guid OrderId,
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    Guid PaymentMethodId);