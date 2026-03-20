namespace Order.Application.Commands.OrderAggregate.CancelOrder;

/// <summary>
/// 取消订单输入DTO
/// </summary>
/// <param name="OrderId">订单ID</param>
/// <param name="CustomerId">客户ID(权限校验)</param>
/// <param name="CancelReason">取消原因(可选)</param>
public record CancelOrderInputDto(
    Guid OrderId,
    Guid CustomerId,
    string? CancelReason);
