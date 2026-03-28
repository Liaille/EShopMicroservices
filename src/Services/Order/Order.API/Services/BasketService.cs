using Order.API.Consumers.Dtos;

namespace Order.API.Services;

public class BasketService(HttpClient httpClient) : IBasketService
{
    public async Task<ShoppingCartDto?> GetBasketAsync(string userName)
    {
        return await httpClient.GetFromJsonAsync<ShoppingCartDto>($"basket/{userName}");
    }
}
