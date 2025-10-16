namespace Basket.API.Basket.GetBasket;

public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;

public record GetBasketResult(ShoppingCart Cart);

public class GetBasketQueryHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        // TODO: 使用仓储从数据库获取购物车
        return new GetBasketResult(new ShoppingCart(query.UserName));
    }
}
