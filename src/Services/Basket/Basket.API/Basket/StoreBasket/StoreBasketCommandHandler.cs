
namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotNull().WithMessage("Cart cannot be null.");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required.");
        RuleFor(x => x.Cart.Items).NotNull().WithMessage("Items cannot be null.");
    }
}

internal class StoreBasketCommandHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        ShoppingCart cart = command.Cart;

        // TODO: 将购物车存储到数据库中(使用Marten的Upsert)
        // TODO: 更新缓存

        return new StoreBasketResult(command.Cart.UserName);
    }
}
