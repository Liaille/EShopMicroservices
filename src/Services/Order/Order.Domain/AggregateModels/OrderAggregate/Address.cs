namespace Order.Domain.AggregateModels.OrderAggregate;

/// <summary>
/// 地址
/// </summary>
public record Address
{
    /// <summary>
    /// 姓
    /// </summary>
    public string FirstName { get; } = default!;

    /// <summary>
    /// 名
    /// </summary>
    public string LastName { get; } = default!;

    /// <summary>
    /// 邮件
    /// </summary>
    public string? Email { get; }

    /// <summary>
    /// 收件地址
    /// </summary>
    public string AddressLine { get; } = default!;

    /// <summary>
    /// 国家
    /// </summary>
    public string Country { get; } = default!;

    /// <summary>
    /// 州/省
    /// </summary>
    public string State { get; } = default!;

    /// <summary>
    /// 邮政编码
    /// </summary>
    public string ZipCode { get; } = default!;

    protected Address() { }

    private Address(string firstName, string lastName, string email, string addressLine, string country, string state, string zipCode)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        AddressLine = addressLine;
        Country = country;
        State = state;
        ZipCode = zipCode;
    }

    public static Address Create(string firstName, string lastName, string email, string addressLine, string country, string state, string zipCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrEmpty(lastName, nameof(lastName));
        EmailValidator.EnsureValidOptional(email);
        ArgumentException.ThrowIfNullOrEmpty(addressLine, nameof(addressLine));

        return new Address(firstName, lastName, email, addressLine, country, state, zipCode);
    }
}
