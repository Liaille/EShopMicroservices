using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data;

/// <summary>
/// 使用代理模式和装饰器模式为购物车仓储添加缓存功能
/// </summary>
/// <param name="repository"></param>
/// <param name="cache"></param>
public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
    public const string CacheKeyPrefix = $"basket:";

    public async Task<ShoppingCart> GetShoppingCartAsync(string userName, CancellationToken cancellationToken = default)
    {
        var cachedCart = await cache.GetStringAsync(GetCacheKey(userName), cancellationToken);
        if (!string.IsNullOrEmpty(cachedCart)) return JsonSerializer.Deserialize<ShoppingCart>(cachedCart)!;
        
        var cart = await repository.GetShoppingCartAsync(userName, cancellationToken);
        await cache.SetStringAsync(GetCacheKey(userName), JsonSerializer.Serialize(cart), cancellationToken);
        return cart;
    }

    public async Task<ShoppingCart> UpsertShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        await repository.UpsertShoppingCartAsync(cart, cancellationToken);
        await cache.SetStringAsync(GetCacheKey(cart.UserName), JsonSerializer.Serialize(cart), cancellationToken);
        return cart;
    }

    public async Task<bool> DeleteShoppingCartAsync(string userName, CancellationToken cancellationToken = default)
    {
        await repository.DeleteShoppingCartAsync(userName, cancellationToken);
        await cache.RemoveAsync(GetCacheKey(userName), cancellationToken);
        return true;
    }

    private static string GetCacheKey(string userName) => $"{CacheKeyPrefix}{userName}";
}
