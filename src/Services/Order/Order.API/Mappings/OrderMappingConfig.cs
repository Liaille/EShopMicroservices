using Order.API.Endpoints.V1.Orders;
using Order.Application.Commands.OrderAggregate.CancelOrder;
using Order.Application.Commands.OrderAggregate.CreateOrder;
using Order.Application.Commands.OrderAggregate.UpdateOrder;
using Order.Application.Commands.OrderAggregate.UpdateOrderStatus;

namespace Order.API.Mappings;

public static class OrderMappingConfig
{
    public static void RegisterOrderMappings(this IServiceCollection services)
    {
        // 手动配置映射Request内部所有字段到xxxInputDto
        TypeAdapterConfig<CreateOrderRequest, CreateOrderCommand>
            .NewConfig()
            .ConstructUsing(request => new CreateOrderCommand(request.Adapt<CreateOrderInputDto>()));

        TypeAdapterConfig<CancelOrderRequest, CancelOrderCommand>
            .NewConfig()
            .ConstructUsing(request => new CancelOrderCommand(request.Adapt<CancelOrderInputDto>()));

        TypeAdapterConfig<UpdateOrderRequest, UpdateOrderCommand>
            .NewConfig()
            .ConstructUsing(req => new UpdateOrderCommand(req.Adapt<UpdateOrderInputDto>()));

        TypeAdapterConfig<UpdateOrderStatusRequest, UpdateOrderStatusCommand>
            .NewConfig()
            .ConstructUsing(req => new UpdateOrderStatusCommand(req.Adapt<UpdateOrderStatusInputDto>()));
    }
}
