namespace EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent @event, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}
