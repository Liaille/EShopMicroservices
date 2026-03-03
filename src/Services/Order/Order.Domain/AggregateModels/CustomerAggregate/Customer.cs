namespace Order.Domain.AggregateModels.CustomerAggregate;

/// <summary>
/// 客户
/// </summary>
public class Customer : AggregateRoot<CustomerId>
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; private set; } = default!;

    private readonly List<PaymentMethod> _paymentMethods = [];
    public IReadOnlyList<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    private Customer() { }

    public static Customer Create(CustomerId id, string name, string email)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));

        Customer customer = new()
        {
            Id = id,
            Name = name,
            Email = email
        };

        return customer;
    }

    public void AddPaymentMethod(PaymentMethod paymentMethod)
    {
        ArgumentNullException.ThrowIfNull(paymentMethod);

        // 校验：同一个客户不能绑定相同的支付方式（按卡号后4位+支付类型）
        if (_paymentMethods.Any(p => p.CardNumber == paymentMethod.CardNumber && p.Id == paymentMethod.Id))
            throw new InvalidOperationException("This payment method has been bound.");
        _paymentMethods.Add(paymentMethod);
    }

    public void RemovePaymentMethod(PaymentMethodId paymentMethodId)
    {
        ArgumentNullException.ThrowIfNull(paymentMethodId);
        PaymentMethod? paymentMethod = _paymentMethods.FirstOrDefault(p => p.Id == paymentMethodId) ?? throw new InvalidOperationException("Payment method not found.");
        _paymentMethods.Remove(paymentMethod);
    }
}
