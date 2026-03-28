using Order.API.Consumers.Dtos;

namespace Order.API.Services;

public interface IBasketService
{
    Task<ShoppingCartDto?> GetBasketAsync(string userName);
}
