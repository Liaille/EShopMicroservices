namespace Order.Application.Dtos.Order;

/// <summary>
/// 订单地址DTO
/// </summary>
/// <param name="FirstName">姓</param>
/// <param name="LastName">名</param>
/// <param name="Email">邮箱</param>
/// <param name="AddressLine">收件地址</param>
/// <param name="Country">国家</param>
/// <param name="State">省/州</param>
/// <param name="ZipCode">邮政编码</param>
[Serializable]
public record AddressDto(
    string FirstName,
    string LastName,
    string Email,
    string AddressLine,
    string Country,
    string State,
    string ZipCode);
