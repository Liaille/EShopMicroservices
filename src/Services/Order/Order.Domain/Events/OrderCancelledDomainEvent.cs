namespace Order.Domain.Events;

/// <summary>
/// 订单取消事件
/// <list type="table">
/// <item>需通知：</item>
/// <item>库存服务：恢复商品库存</item>
/// <item>支付服务：发起退款</item>
/// <item>消息服务：发送取消通知</item>
/// </list>
/// </summary>
public sealed class OrderCancelledDomainEvent : DomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderCancelledDomainEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }
}
