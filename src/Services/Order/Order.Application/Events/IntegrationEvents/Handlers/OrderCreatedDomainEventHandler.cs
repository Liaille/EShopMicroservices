namespace Order.Application.Events.IntegrationEvents.Handlers;

public class OrderCreatedDomainEventHandler(ILogger<OrderCreatedDomainEventHandler> logger) : INotificationHandler<OrderCreatedDomainEvent>
{
    public Task Handle(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Domain Event handled: {DomainEvent}", domainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
