namespace Order.Domain.Events;

/// <summary>
/// 订单支付事件
/// <list type="table">
/// <item>需通知：</item>
/// <item>仓库服务：生成出库单</item>
/// <item>财务服务：记录交易流水</item>
/// <item>消息服务：发送支付成功通知</item>
/// </list>
/// </summary>
public sealed class OrderPaidDistributedEvent : DomainEvent, IDistributedDomainEvent
{
    public AggregateModels.OrderAggregate.Order Order { get; }

    public string? PaymentRecordId { get; }

    public OrderPaidDistributedEvent(AggregateModels.OrderAggregate.Order order, string? paymentRecordId) : base()
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        PaymentRecordId = paymentRecordId;
    }
}
