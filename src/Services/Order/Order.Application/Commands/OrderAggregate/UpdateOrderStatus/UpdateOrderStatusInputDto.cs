namespace Order.Application.Commands.OrderAggregate.UpdateOrderStatus;

/// <summary>
/// 更新订单状态输入DTO
/// </summary>
/// <param name="OrderId">订单ID</param>
/// <param name="TargetStatus">目标状态</param>
/// <param name="PaymentRecordId">支付记录ID(仅支付状态需要)</param>
public record UpdateOrderStatusInputDto(
    Guid OrderId,
    OrderStatus TargetStatus,
    string? PaymentRecordId);
