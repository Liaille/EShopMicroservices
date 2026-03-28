namespace Order.API.Consumers.Dtos;

public record ShoppingCartDto(
    string UserName,
    IReadOnlyList<ShoppingCartItemDto> Items,
    decimal TotalPrice);
