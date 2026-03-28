namespace Order.API.Consumers.Dtos;

public record ShoppingCartItemDto(
    Guid ProductId,
    string ProductName,
    string Color,
    int Quantity,
    decimal Price);
