using Order.API.Consumers.Dtos;

namespace Order.API.Services;

public class BasketService(HttpClient httpClient) : IBasketService
{
    public async Task<ShoppingCartDto?> GetBasketAsync(string userName)
    {
        var response = await httpClient.GetFromJsonAsync<GetBasketResponseDto>($"basket/{userName}");

        if (response == null) return null;

        return response.Cart;
    }
}
