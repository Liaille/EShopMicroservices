using Basket.API.Basket.CheckoutBasket;

namespace Basket.API.Mappings;

public static class BasketMappingConfig
{
    public static void AddRequestMappings(this IServiceCollection services)
    {
        TypeAdapterConfig<CheckoutBasketRequest, CheckoutBasketCommand>
            .NewConfig()
            .ConstructUsing(request => new CheckoutBasketCommand(request.Adapt<CheckoutBasketInputDto>()));
    }
}
