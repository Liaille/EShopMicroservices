using FluentValidation;

namespace Order.Application.Dtos.OrderAggregate;

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    /// <summary>
    /// 最小购买数量
    /// </summary>
    private const int MinQuantity = 1;

    /// <summary>
    /// 最大购买数量(单商品)
    /// </summary>
    private const int MaxQuantity = 999;

    /// <summary>
    /// 单价总位数
    /// </summary>
    private const int PricePrecision = 10;

    /// <summary>
    /// 单价小数位数
    /// </summary>
    private const int PriceScale = 2;

    public OrderItemDtoValidator()
    {
        RuleFor(oi => oi.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("Product ID cannot be empty.");

        RuleFor(oi => oi.Quantity)
            .GreaterThan(MinQuantity - 1)
            .WithMessage($"The purchase quantity must be greater than {MinQuantity - 1}")
            .LessThan(MaxQuantity)
            .WithMessage($"The purchase quantity of a single product cannot exceed {MaxQuantity} pieces");

        RuleFor(oi => oi.Price)
            .GreaterThan(0)
            .WithMessage("The unit price of the product must be greater than 0")
            .PrecisionScale(PricePrecision, PriceScale, true)
            .WithMessage($"The unit price of the product can be kept to a maximum of {PriceScale} decimal places, The total number of digits does not exceed {PricePrecision} digits (e.g. 9999999.99)");
    }
}
