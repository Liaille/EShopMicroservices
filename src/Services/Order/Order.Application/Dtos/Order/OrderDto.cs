using Order.Domain.AggregateModels.OrderAggregate;

namespace Order.Application.Dtos.Order;

/// <summary>
/// 订单DTO
/// </summary>
/// <param name="Id">订单Id</param>
/// <param name="OrderDate">订单创建日期</param>
/// <param name="CustomerId">客户Id</param>
/// <param name="OrderName">订单名称</param>
/// <param name="ShippingAddress">收货地址(商品寄送地址)</param>
/// <param name="BillingAddress">账单地址(支付验证、发票开具地址)</param>
/// <param name="PaymentMethodId">支付方式Id</param>
/// <param name="Status">订单状态</param>
/// <param name="OrderItems">订单项列表</param>
[Serializable]
public record OrderDto(
    Guid Id,
    DateTime OrderDate,
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    Guid PaymentMethodId,
    OrderStatus Status,
    List<OrderItemDto> OrderItems);
