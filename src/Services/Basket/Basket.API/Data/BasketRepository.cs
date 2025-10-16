using Basket.API.Exceptions;

namespace Basket.API.Data;

public class BasketRepository(IDocumentSession session) : IBasketRepository
{
    public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
    {
        return await session.LoadAsync<ShoppingCart>(userName, cancellationToken) ?? throw new BasketNotFoundException(userName);
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        session.Store(cart);
        await session.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        session.Delete<ShoppingCart>(userName);
        await session.SaveChangesAsync(cancellationToken);
        return true;
    }
}
