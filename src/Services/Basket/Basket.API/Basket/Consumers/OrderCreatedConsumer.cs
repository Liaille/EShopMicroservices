using EventBus.Events.Order;
using MassTransit;

namespace Basket.API.Basket.Consumers;

public class OrderCreatedConsumer(IBasketRepository repository) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        var eventMessage = context.Message;

        await repository.DeleteShoppingCartAsync(eventMessage.UserName, context.CancellationToken);
    }
}
