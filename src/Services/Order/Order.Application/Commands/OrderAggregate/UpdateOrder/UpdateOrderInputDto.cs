namespace Order.Application.Commands.OrderAggregate.UpdateOrder;

/// <summary>
/// 更新订单基础信息输入DTO
/// </summary>
/// <param name="OrderId"></param>
/// <param name="CustomerId"></param>
/// <param name="OrderName"></param>
/// <param name="ShippingAddress"></param>
/// <param name="BillingAddress"></param>
/// <param name="PaymentMethodId"></param>
public record UpdateOrderInputDto(
    Guid OrderId,
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    Guid PaymentMethodId);
