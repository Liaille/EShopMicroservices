namespace BuildingBlocks.EventBus.Events.Basket;

/// <summary>
/// 购物车结账集成事件
/// <para>由购物车服务(Basket.API)在用户提交订单时发布</para>
/// <para>订单服务(Order.API)订阅并消费，用于创建订单</para>
/// <para>注意：商品明细通过调用购物车API获取，不包含在事件中</para>
/// </summary>
public record BasketCheckoutIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// 用户名（用于唯一标识用户购物车）
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// 客户ID（强类型关联用户）
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// 订单总价
    /// </summary>
    public decimal TotalPrice { get; init; }

    /// <summary>
    /// 订单名称/标题
    /// </summary>
    public string OrderName { get; init; } = string.Empty;

    #region 收货地址信息
    /// <summary>
    /// 收货人姓名
    /// </summary>
    public string ShippingFirstName { get; init; } = string.Empty;

    /// <summary>
    /// 收货人姓氏
    /// </summary>
    public string ShippingLastName { get; init; } = string.Empty;

    /// <summary>
    /// 收货人邮箱
    /// </summary>
    public string ShippingEmail { get; init; } = string.Empty;

    /// <summary>
    /// 收货地址
    /// </summary>
    public string ShippingAddressLine { get; init; } = string.Empty;

    /// <summary>
    /// 收货国家
    /// </summary>
    public string ShippingCountry { get; init; } = string.Empty;

    /// <summary>
    /// 收货省份/州
    /// </summary>
    public string ShippingState { get; init; } = string.Empty;

    /// <summary>
    /// 收货邮编
    /// </summary>
    public string ShippingZipCode { get; init; } = string.Empty;
    #endregion

    #region 账单地址信息
    /// <summary>
    /// 账单人姓名
    /// </summary>
    public string BillingFirstName { get; init; } = string.Empty;

    /// <summary>
    /// 账单人姓氏
    /// </summary>
    public string BillingLastName { get; init; } = string.Empty;

    /// <summary>
    /// 账单人邮箱
    /// </summary>
    public string BillingEmail { get; init; } = string.Empty;

    /// <summary>
    /// 账单地址
    /// </summary>
    public string BillingAddressLine { get; init; } = string.Empty;

    /// <summary>
    /// 账单国家
    /// </summary>
    public string BillingCountry { get; init; } = string.Empty;

    /// <summary>
    /// 账单省份/州
    /// </summary>
    public string BillingState { get; init; } = string.Empty;

    /// <summary>
    /// 账单邮编
    /// </summary>
    public string BillingZipCode { get; init; } = string.Empty;
    #endregion

    /// <summary>
    /// 支付方式ID
    /// </summary>
    public Guid PaymentMethodId { get; init; }
}
