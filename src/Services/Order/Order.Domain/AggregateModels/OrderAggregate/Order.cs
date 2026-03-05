namespace Order.Domain.AggregateModels.OrderAggregate;

public class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderItem> _orderItems = [];

    /// <summary>
    /// 订单项只读列表
    /// </summary>
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// 订单日期时间
    /// </summary>
    public DateTime OrderDate { get; private set; }

    /// <summary>
    /// 顾客Id
    /// </summary>
    public CustomerId CustomerId { get; private set; } = default!;

    /// <summary>
    /// 订单名称
    /// </summary>
    public OrderName OrderName { get; private set; } = default!;

    /// <summary>
    /// 收货地址(商品寄送地址)
    /// </summary>
    public Address ShippingAddress { get; private set; } = default!;

    /// <summary>
    /// 账单地址(支付验证、发票开具地址)
    /// </summary>
    public Address BillingAddress { get; private set; } = default!;

    /// <summary>
    /// 支付方式Id
    /// </summary>
    public PaymentMethodId PaymentMethodId { get; private set; } = default!;

    /// <summary>
    /// 订单状态
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// 订单总价
    /// </summary>
    public decimal TotalPrice => OrderItems.Sum(x => x.Price * x.Quantity);

    /// <summary>
    /// 私有化无参构造函数，强制通过Create方法创建
    /// </summary>
    private Order() { }

    /// <summary>
    /// 创建订单（初始化状态为Submitted）
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <param name="customerId">客户ID</param>
    /// <param name="orderName">订单名称</param>
    /// <param name="shippingAddress">配送地址</param>
    /// <param name="billingAddress">账单地址</param>
    /// <param name="paymentMethodId">支付方式ID</param>
    /// <returns>订单聚合根</returns>
    public static Order Create(
        OrderId id,
        CustomerId customerId,
        OrderName orderName,
        Address shippingAddress,
        Address billingAddress,
        PaymentMethodId paymentMethodId)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(customerId, nameof(customerId));
        ArgumentNullException.ThrowIfNull(orderName, nameof(orderName));
        ArgumentNullException.ThrowIfNull(shippingAddress, nameof(shippingAddress));
        ArgumentNullException.ThrowIfNull(billingAddress, nameof(billingAddress));
        ArgumentNullException.ThrowIfNull(paymentMethodId, nameof(paymentMethodId));

        var order = new Order
        {
            Id = id,
            OrderDate = DateTime.UtcNow,
            CustomerId = customerId,
            OrderName = orderName,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            PaymentMethodId = paymentMethodId,
            Status = OrderStatus.Submitted // 订单创建时初始状态为“已提交”
        };

        // 发布订单创建领域事件
        order.AddDistributedEvent(new OrderCreatedEvent(order));
        return order;
    }

    /// <summary>
    /// 更新订单基础信息（不含状态流转）
    /// </summary>
    /// <param name="orderName">订单名称</param>
    /// <param name="shippingAddress">配送地址</param>
    /// <param name="billingAddress">账单地址</param>
    /// <param name="paymentMethodId">支付方式ID</param>
    public void Update(
        OrderName orderName,
        Address shippingAddress,
        Address billingAddress,
        PaymentMethodId paymentMethodId)
    {
        // 参数校验
        ArgumentNullException.ThrowIfNull(orderName, nameof(orderName));
        ArgumentNullException.ThrowIfNull(shippingAddress, nameof(shippingAddress));
        ArgumentNullException.ThrowIfNull(billingAddress, nameof(billingAddress));
        ArgumentNullException.ThrowIfNull(paymentMethodId, nameof(paymentMethodId));

        // 仅允许未取消/未发货的订单更新基础信息
        if (Status is OrderStatus.Cancelled or OrderStatus.Shipped)
            throw new DomainException("已取消/已发货的订单不允许修改基础信息");

        OrderName = orderName;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        PaymentMethodId = paymentMethodId;

        AddDistributedEvent(new OrderUpdatedEvent(this));
    }

    /// <summary>
    /// 新增订单项
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="quantity">数量</param>
    /// <param name="price">单价</param>
    public void AddOrderItem(ProductId productId, int quantity, decimal price)
    {
        ArgumentNullException.ThrowIfNull(productId, nameof(productId));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        // 仅允许未取消/未支付的订单添加订单项
        if (Status is OrderStatus.Cancelled or OrderStatus.Paid or OrderStatus.Shipped)
        {
            throw new DomainException("已取消/已支付/已发货的订单不允许添加订单项");
        }

        var orderItem = new OrderItem(Id, productId, quantity, price);
        _orderItems.Add(orderItem);
    }

    /// <summary>
    /// 移除订单项
    /// </summary>
    /// <param name="productId">商品ID</param>
    public void RemoveOrderItem(ProductId productId)
    {
        ArgumentNullException.ThrowIfNull(productId, nameof(productId));

        // 仅允许未取消/未支付的订单移除订单项
        if (Status is OrderStatus.Cancelled or OrderStatus.Paid or OrderStatus.Shipped)
            throw new DomainException("已取消/已支付/已发货的订单不允许移除订单项");

        var orderItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (orderItem is not null)
            _orderItems.Remove(orderItem);
    }

    #region 订单状态流转方法（核心业务规则）
    /// <summary>
    /// 标记订单为待验证状态
    /// </summary>
    public void MarkAsAwaitingValidation()
    {
        // 仅允许从“已提交”状态流转到“待验证”
        if (Status != OrderStatus.Submitted)
        {
            throw new DomainException($"订单状态{Status}不允许流转到待验证");
        }

        Status = OrderStatus.AwaitingValidation;
        AddDistributedEvent(new OrderStatusChangedEvent(this, Status));
    }

    /// <summary>
    /// 标记订单为库存已确认状态
    /// </summary>
    public void MarkAsStockConfirmed()
    {
        // 仅允许从“待验证”状态流转到“库存已确认”
        if (Status != OrderStatus.AwaitingValidation)
        {
            throw new DomainException($"订单状态{Status}不允许流转到库存已确认");
        }

        Status = OrderStatus.StockConfirmed;
        AddDistributedEvent(new OrderStatusChangedEvent(this, Status));
    }

    /// <summary>
    /// 标记订单为已支付状态
    /// </summary>
    /// <param name="paymentRecordId">支付记录ID（可选，如第三方支付流水号）</param>
    public void MarkAsPaid(string? paymentRecordId = null)
    {
        // 仅允许从“库存已确认”状态流转到“已支付”
        if (Status != OrderStatus.StockConfirmed)
        {
            throw new DomainException($"订单状态{Status}不允许流转到已支付");
        }

        Status = OrderStatus.Paid;
        AddDistributedEvent(new OrderPaidEvent(this, paymentRecordId));
        AddDistributedEvent(new OrderStatusChangedEvent(this, Status));
    }

    /// <summary>
    /// 标记订单为已发货状态
    /// </summary>
    public void MarkAsShipped()
    {
        // 仅允许从“已支付”状态流转到“已发货”
        if (Status != OrderStatus.Paid)
        {
            throw new DomainException($"订单状态{Status}不允许流转到已发货");
        }

        Status = OrderStatus.Shipped;
        AddDistributedEvent(new OrderShippedEvent(this));
        AddDistributedEvent(new OrderStatusChangedEvent(this, Status));
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    public void Cancel()
    {
        // 仅允许未发货的订单取消（已提交/待验证/库存已确认/已支付）
        if (Status == OrderStatus.Shipped)
        {
            throw new DomainException("已发货的订单不允许取消");
        }

        Status = OrderStatus.Cancelled;
        AddDistributedEvent(new OrderCancelledEvent(this));
        AddDistributedEvent(new OrderStatusChangedEvent(this, Status));
    }
    #endregion
}
