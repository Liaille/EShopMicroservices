namespace Basket.API.Data;

public interface IBasketRepository
{
    Task<ShoppingCart> GetShoppingCartAsync(string userName, CancellationToken cancellationToken = default);

    Task<ShoppingCart> UpsertShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default);

    Task<bool> DeleteShoppingCartAsync(string userName, CancellationToken cancellationToken = default);
}
