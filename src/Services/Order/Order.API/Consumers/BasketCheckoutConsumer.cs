using EventBus.Events.Basket;
using Order.API.Mappings;
using Order.API.Services;
using Order.Application.Commands.OrderAggregate.CreateOrder;

namespace Order.API.Consumers;

public sealed class BasketCheckoutConsumer(
    IBasketService basketService,
    IMediator mediator,
    ILogger<BasketCheckoutConsumer> logger) : IConsumer<BasketCheckoutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
    {
        var eventMessage = context.Message;

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Starting to process the BasketCheckoutIntegrationEvent for user: {UserName}", eventMessage.UserName);

        try
        {
            var shoppingCart = await basketService.GetBasketAsync(eventMessage.UserName);

            if (shoppingCart is null || shoppingCart.Items.Count == 0)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError("The shopping cart is empty, unable to create an order for user: {UserName}", eventMessage.UserName);
                return;
            }

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Successfully obtained shopping cart | User: {UserName} | Total amount: {TotalPrice}", eventMessage.UserName, eventMessage.TotalPrice);

            // 构建创建订单命令 (将外部DTO转换为应用层需要的参数)
            var createOrderInputDto = eventMessage.ToCreateOrderInputDto(shoppingCart);
            var createOrderCommand = new CreateOrderCommand(createOrderInputDto);

            await mediator.Send(createOrderCommand);
        }
        catch (HttpRequestException ex)
        {
            if (logger.IsEnabled(LogLevel.Error))
                logger.LogError(ex, "Call Basket.API failed: {UserName}", eventMessage.UserName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to settle the BasketCheckoutIntegrationEvent for user: {UserName}", eventMessage.UserName);
            throw; // MassTransit自动重试
        }
    }
}
