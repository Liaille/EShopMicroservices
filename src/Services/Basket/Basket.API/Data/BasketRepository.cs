using Basket.API.Exceptions;

namespace Basket.API.Data;

public class BasketRepository(IDocumentSession session) : IBasketRepository
{
    public async Task<ShoppingCart> GetShoppingCartAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await session.LoadAsync<ShoppingCart>(userName, cancellationToken) ?? throw new BasketNotFoundException(userName);
    }

    public async Task<ShoppingCart> UpsertShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        session.Store(cart);
        await session.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<bool> DeleteShoppingCartAsync(string userName, CancellationToken cancellationToken = default)
    {
        session.Delete<ShoppingCart>(userName);
        await session.SaveChangesAsync(cancellationToken);
        return true;
    }
}
