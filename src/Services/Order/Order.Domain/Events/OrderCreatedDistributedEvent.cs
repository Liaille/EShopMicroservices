namespace Order.Domain.Events;

/// <summary>
/// 订单创建事件
/// <list type="table">
/// <item>需通知：</item>
/// <item>库存服务：扣减商品库存</item>
/// <item>支付服务：生成支付单</item>
/// <item>消息服务：发送下单成功通知</item>
/// </list>
/// </summary>
public sealed class OrderCreatedDistributedEvent : DomainEvent, IDistributedDomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderCreatedDistributedEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public OrderCreatedDistributedEvent(Guid eventId, DateTime occurredOn, AggregateModels.OrderAggregate.Order order) : base(eventId, occurredOn)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        if (occurredOn.Kind != DateTimeKind.Utc) throw new ArgumentException("The occurrence time of the event must be in UTC.", nameof(occurredOn));
    }
}
