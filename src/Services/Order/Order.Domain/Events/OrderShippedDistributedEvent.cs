namespace Order.Domain.Events;

/// <summary>
/// 订单发货事件
/// <list type="table">
/// <item>需通知：</item>
/// <item>物流服务：创建物流单</item>
/// <item>消息服务：发送发货通知</item>
/// <item>售后服务：初始化售后权限</item>
/// </list>
/// </summary>
public sealed class OrderShippedDistributedEvent : DomainEvent, IDistributedDomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public OrderShippedDistributedEvent(AggregateModels.OrderAggregate.Order order) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }
}
