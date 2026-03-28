using BuildingBlocks.EventBus.Events.Basket;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(CheckoutBasketInputDto Input) : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.Input)
            .NotNull().WithMessage("Input parameter cannot be null.");

        RuleFor(x => x.Input.UserName)
            .NotEmpty().WithMessage("User name cannot be empty.");
    }
}

public class CheckoutBasketCommandHandler(IBasketRepository repository, IPublishEndpoint publishEndpoint) : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await repository.GetShoppingCartAsync(command.Input.UserName, cancellationToken);
        if (basket == null) return new CheckoutBasketResult(false);

        var eventMessage = command.Input.Adapt<BasketCheckoutIntegrationEvent>();

        await publishEndpoint.Publish(eventMessage, cancellationToken);

        return new CheckoutBasketResult(true);
    }
}
