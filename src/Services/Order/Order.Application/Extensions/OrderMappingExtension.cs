namespace Order.Application.Extensions;

/// <summary>
/// 订单聚合下领域模型转应用层DTO映射扩展类
/// </summary>
public static class OrderMappingExtension
{
    public static OrderDto ToOrderDto(this Domain.AggregateModels.OrderAggregate.Order order)
    {
        ArgumentNullException.ThrowIfNull(order, nameof(order));

        return new OrderDto(
            Id: order.Id.Value,
            OrderDate: order.OrderDate,
            CustomerId: order.CustomerId.Value,
            OrderName: order.OrderName.Value,
            ShippingAddress: order.ShippingAddress.ToAddressDto(),
            BillingAddress: order.BillingAddress.ToAddressDto(),
            PaymentMethodId: order.PaymentMethodId.Value,
            Status: order.Status.ToString(),
            OrderItems: [.. order.OrderItems.Select(oi => oi.ToOrderItemDto())]);
    }

    public static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(orderItem, nameof(orderItem));

        return new OrderItemDto(orderItem.ProductId.Value, orderItem.Quantity, orderItem.Price);
    }

    public static AddressDto ToAddressDto(this Address address)
    {
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        return new AddressDto(
            FirstName: address.FirstName,
            LastName: address.LastName,
            Country: address.Country,
            State: address.State,
            AddressLine: address.AddressLine,
            ZipCode: address.ZipCode,
            Email: address.Email);
    }
}
