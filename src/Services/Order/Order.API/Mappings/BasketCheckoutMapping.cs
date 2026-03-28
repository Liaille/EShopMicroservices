using BuildingBlocks.EventBus.Events.Basket;
using Order.API.Consumers.Dtos;
using Order.Application.Commands.OrderAggregate.CreateOrder;

namespace Order.API.Mappings;

public static class BasketCheckoutMapping
{
    // 映射创建订单输入
    public static CreateOrderInputDto ToCreateOrderInputDto(
        this BasketCheckoutIntegrationEvent @event,
        ShoppingCartDto cartDto)
    {
        return new CreateOrderInputDto(
            CustomerId: @event.CustomerId,
            OrderName: $"Order_{@event.UserName}",
            ShippingAddress: @event.ToShippingAddressDto(),
            BillingAddress: @event.ToBillingAddressDto(),
            PaymentMethodId: @event.PaymentMethodId,
            OrderItems: cartDto.ToOrderItemsDto());
    }

    public static AddressDto ToShippingAddressDto(this BasketCheckoutIntegrationEvent @event)
    {
        return new AddressDto(
            FirstName: @event.ShippingFirstName,
            LastName: @event.ShippingLastName,
            Country: @event.ShippingCountry,
            State: @event.ShippingState,
            AddressLine: @event.ShippingAddressLine,
            ZipCode: @event.ShippingZipCode,
            Email: @event.ShippingEmail);
    }

    // 映射账单地址
    public static AddressDto ToBillingAddressDto(this BasketCheckoutIntegrationEvent @event)
    {
        return new AddressDto(
            FirstName: @event.BillingFirstName,
            LastName: @event.BillingLastName,
            Country: @event.BillingCountry,
            State: @event.BillingState,
            AddressLine: @event.BillingAddressLine,
            ZipCode: @event.BillingZipCode,
            Email: @event.BillingEmail);
    }

    // 映射订单项 
    public static IReadOnlyList<OrderItemDto> ToOrderItemsDto(this ShoppingCartDto cartDto)
    {
        return [.. cartDto.Items.Select(i => new OrderItemDto(
            ProductId: i.ProductId,
            Quantity: i.Quantity,
            Price: i.Price
        ))];
    }
}
