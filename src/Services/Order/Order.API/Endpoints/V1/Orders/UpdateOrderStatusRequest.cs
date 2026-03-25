using Order.Domain.AggregateModels.OrderAggregate;

namespace Order.API.Endpoints.V1.Orders;

public record UpdateOrderStatusRequest(
    Guid OrderId,
    OrderStatus TargetStatus,
    string? PaymentRecordId);
