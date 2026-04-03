namespace Order.Application.Events.DomainEvents.Handlers;

public class OrderCreatedDomainEventHandler(
    IOrderDbContext dbContext,
    IEventBus eventBus, 
    IFeatureManager featureManager, 
    ILogger<OrderCreatedDomainEventHandler> logger) : INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Domain Event handled: {DomainEvent}", domainEvent.GetType().Name);

        if (await featureManager.IsEnabledAsync("OrderFullfilment"))
        {
            var order = domainEvent.Order;
            var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == order.CustomerId, cancellationToken);

            var integrationEvent = new OrderCreatedIntegrationEvent
            {
                OrderId = order.Id.Value,
                UserName = customer?.Name ?? string.Empty,
                CustomerId = order.CustomerId.Value,
                TotalPrice = order.TotalPrice
            };

            await eventBus.PublishAsync(integrationEvent, cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Integration Event published: {IntegrationEvent}", integrationEvent.GetType().Name);
        }
    }
}
