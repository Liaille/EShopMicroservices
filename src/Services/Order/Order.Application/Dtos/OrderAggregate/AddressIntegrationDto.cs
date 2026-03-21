namespace Order.Application.Dtos.OrderAggregate;

/// <summary>
/// 地址集成DTO (跨服务传输)
/// </summary>
/// <param name="FirstName">姓</param>
/// <param name="LastName">名</param>
/// <param name="Country">国家</param>
/// <param name="State">省/州</param>
/// <param name="AddressLine">收件地址</param>
/// <param name="ZipCode">邮政编码</param>
/// <param name="Email">邮箱 (可选)</param>
public record AddressIntegrationDto(
    string FirstName,
    string LastName,
    string Country,
    string State,
    string AddressLine,
    string ZipCode,
    string? Email)
{
    /// <summary>
    /// 从领域层 <see cref="Address"/> 转换为 <see cref="AddressIntegrationDto"/>
    /// </summary>
    /// <param name="address">领域层地址对象</param>
    /// <returns><see cref="AddressIntegrationDto"/></returns>
    /// <exception cref="ArgumentNullException">领域对象为空时抛出</exception>
    public static AddressIntegrationDto FromDomain(Address address)
    {
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        return new AddressIntegrationDto(
            FirstName: address.FirstName,
            LastName: address.LastName,
            Country: address.Country,
            State: address.State,
            AddressLine: address.AddressLine,
            ZipCode: address.ZipCode,
            Email: address.Email);
    }

    /// <summary>
    /// 仅校验跨服务传输必需字段
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FirstName) 
            && !string.IsNullOrWhiteSpace(LastName) 
            && !string.IsNullOrWhiteSpace(AddressLine)
            && !string.IsNullOrWhiteSpace(ZipCode);
    }
};
