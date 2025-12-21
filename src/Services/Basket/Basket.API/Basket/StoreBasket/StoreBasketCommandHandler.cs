using Discount.Grpc;
using JasperFx.Events.Daemon;

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

internal class StoreBasketCommandHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountService) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        // 调用折扣Grpc服务为购物车中的每一项应用优惠券
        await DeductDiscount(command.Cart, cancellationToken);

        // 将数据库和缓存操作交由仓储处理
        await repository.UpsertShoppingCartAsync(command.Cart, cancellationToken);

        return new StoreBasketResult(command.Cart.UserName);
    }

    /// <summary>
    /// 扣除优惠券折扣
    /// </summary>
    /// <param name="cart"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            var coupon = await discountService.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
            item.Price -= coupon.Amount;
        }
    }
}
