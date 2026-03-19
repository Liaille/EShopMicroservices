namespace Order.Application.Commands.OrderAggregate.CreateOrder;

public record CreateOrderInputDto(
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    Guid PaymentMethodId,
    IReadOnlyList<OrderItemDto> OrderItems);
