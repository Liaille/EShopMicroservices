using Order.Domain.Events;

namespace Order.Application.Events.IntegrationEvents;

public sealed class OrderCreatedIntegrationEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// 事件发生时间 (UTC)
    /// </summary>
    public DateTime OccurredOn { get; init; }

    /// <summary>
    /// 订单ID
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// 客户ID
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// 订单名称
    /// </summary>
    public string OrderName { get; init; } = string.Empty;

    /// <summary>
    /// 订单日期时间 (UTC)
    /// </summary>
    public DateTime OrderDate { get; init; }

    /// <summary>
    /// 订单商品项列表
    /// </summary>
    public List<OrderItemIntegrationDto> OrderItems { get; init; } = [];

    /// <summary>
    /// 订单总价
    /// </summary>
    public decimal TotalPrice { get; init; }

    /// <summary>
    /// 支付方式ID
    /// </summary>
    public Guid PaymentMethodId { get; init; }

    /// <summary>
    /// 配送地址
    /// </summary>
    public AddressIntegrationDto ShippingAddress { get; init; } = default!;

    /// <summary>
    /// 账单地址 (支付验证、发票开具地址)
    /// </summary>
    public AddressIntegrationDto BillingAddress { get; init; } = default!;

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status {  get; init; }

    /// <summary>
    /// 从领域层 <see cref="OrderCreatedDomainEvent"/> 转换为 <see cref="OrderCreatedIntegrationEvent"/>
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    /// <returns><see cref="OrderCreatedIntegrationEvent"/></returns>
    /// <exception cref="ArgumentNullException">领域对象为空时抛出</exception>
    public static OrderCreatedIntegrationEvent FromDomain(OrderCreatedDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent, nameof(domainEvent));
        ArgumentNullException.ThrowIfNull(domainEvent.Order, nameof(domainEvent.Order));

        return new OrderCreatedIntegrationEvent
        {
            EventId = domainEvent.EventId,
            OccurredOn = domainEvent.OccurredOn,

            OrderId = domainEvent.Order.Id.Value,
            CustomerId = domainEvent.Order.CustomerId.Value,
            OrderDate = domainEvent.Order.OrderDate,
            OrderName = domainEvent.Order.OrderName.Value,
            Status = domainEvent.Order.Status,
            PaymentMethodId = domainEvent.Order.PaymentMethodId.Value,
            TotalPrice = domainEvent.Order.TotalPrice,

            ShippingAddress = AddressIntegrationDto.FromDomain(domainEvent.Order.ShippingAddress),
            BillingAddress = AddressIntegrationDto.FromDomain(domainEvent.Order.BillingAddress),

            OrderItems = [.. domainEvent.Order.OrderItems
            .Select(OrderItemIntegrationDto.FromDomain)
            .Where(dto => dto.IsValid())]
        };
    }
}
