namespace Order.Application.Dtos.OrderAggregate;

public record OrderDto(
    Guid Id,
    DateTime OrderDate,
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    Guid PaymentMethodId,
    string Status,
    IReadOnlyList<OrderItemDto> OrderItems);
