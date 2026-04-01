using EventBus.Abstractions;
using MassTransit;

namespace Order.Infrastructure.EventBus;

public class MassTransitEventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
    public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        await publishEndpoint.Publish(@event, cancellationToken);
    }
}
