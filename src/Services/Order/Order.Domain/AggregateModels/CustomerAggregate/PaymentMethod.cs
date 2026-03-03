namespace Order.Domain.AggregateModels.CustomerAggregate;

public class PaymentMethod : Entity<PaymentMethodId>
{
    /// <summary>
    /// 银行卡类型Id，如1=Visa、2=MasterCard、3=银联等
    /// </summary>
    public int CardTypeId { get; private set; }

    /// <summary>
    /// 卡号(脱敏存储，仅保留后4位)
    /// </summary>
    public string CardNumber { get; private set; } = default!;

    /// <summary>
    /// 持卡人姓名
    /// </summary>
    public string CardHolderName { get; private set; } = default!;

    /// <summary>
    /// 到期时间(UTC时间)
    /// </summary>
    public DateTime Expiration { get; private set; }

    /// <summary>
    /// 卡类型（导航属性，非敏感信息）
    /// </summary>
    public CardType CardType { get; private set; } = default!;

    private PaymentMethod() { }

    /// <summary>
    /// 创建支付方式实体（仅存储非敏感信息）
    /// </summary>
    /// <param name="cardTypeId">银行卡类型ID</param>
    /// <param name="cardNumber">完整卡号（内部自动脱敏）</param>
    /// <param name="cardHolderName">持卡人姓名</param>
    /// <param name="expiration">到期时间（UTC）</param>
    /// <param name="cardType">银行卡类型实体</param>
    /// <returns>支付方式实体</returns>
    public static PaymentMethod Create(int cardTypeId, string cardNumber, string cardHolderName, DateTime expiration, CardType cardType)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new DomainException("CardNumber cannot be empty.");
        if (string.IsNullOrWhiteSpace(cardHolderName))
            throw new DomainException("CardHolderName cannot be empty.");
        if (expiration < DateTime.UtcNow)
            throw new DomainException("Expiration date cannot be in the past.");
        if (cardType == null)
            throw new DomainException("CardType cannot be null.");
        if (cardTypeId != cardType.Id)
            throw new DomainException("CardTypeId does not match CardType.Id.");

        // 卡号脱敏：仅保留后4位
        string maskedCardNumber = cardNumber.Length >= 4 ? cardNumber[^4..] : throw new DomainException("CardNumber must be at least 4 digits long.");

        return new PaymentMethod
        {
            Id = PaymentMethodId.Create(Guid.NewGuid()),
            CardTypeId = cardTypeId,
            CardNumber = maskedCardNumber,
            CardHolderName = cardHolderName,
            Expiration = expiration.ToUniversalTime(),
            CardType = cardType
        };
    }

    /// <summary>
    /// 静态方法：校验安全码（仅支付时临时使用，不存储）
    /// </summary>
    /// <param name="securityNumber">用户临时输入的安全码</param>
    /// <returns>是否合法</returns>
    public static bool ValidateSecurityNumber(string securityNumber)
    {
        // 安全码格式校验：3-4位数字
        return !string.IsNullOrWhiteSpace(securityNumber)
            && securityNumber.Length is 3 or 4
            && securityNumber.All(char.IsDigit);
    }

    /// <summary>
    /// 校验支付方式是否匹配（用于重复绑定校验）
    /// </summary>
    public bool IsEqualTo(int cardTypeId, string fullCardNumber, DateTime expiration)
    {
        // 用脱敏后的卡号后4位 + 类型 + 有效期校验
        string fullCardLast4 = fullCardNumber.Length >= 4 ? fullCardNumber[^4..] : string.Empty;

        return CardTypeId == cardTypeId
            && CardNumber == fullCardLast4
            && Expiration == expiration.ToUniversalTime();
    }
}
