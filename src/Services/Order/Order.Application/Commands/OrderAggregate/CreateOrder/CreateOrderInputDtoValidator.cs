using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.CreateOrder;

public class CreateOrderInputDtoValidator : AbstractValidator<CreateOrderInputDto>
{
    public CreateOrderInputDtoValidator(AddressDtoValidator addressValidator, OrderItemDtoValidator orderItemValidator)
    {
        RuleFor(dto => dto.CustomerId)
            .NotEqual(Guid.Empty).WithMessage("Customer ID cannot be empty.");

        RuleFor(dto => dto.OrderName)
            .NotEmpty().WithMessage("Order name cannot be empty.")
            .MaximumLength(100).WithMessage("The length of the order name cannot exceed 100 characters.")
            .Matches(@"^[^<>/\\|""*?]+$").WithMessage("The order name cannot contain special characters (<>/\\ \\ | \"\" *?)");

        RuleFor(dto => dto.PaymentMethodId)
            .NotEqual(Guid.Empty).WithMessage("Payment method ID cannot be empty.");

        RuleFor(dto => dto.ShippingAddress)
            .NotNull().WithMessage("Shipping address cannot be empty.")
            .SetValidator(addressValidator);

        RuleFor(dto => dto.BillingAddress)
            .NotNull().WithMessage("Billing address cannot be empty.")
            .SetValidator(addressValidator);

        RuleFor(dto => dto.OrderItems)
            .NotNull().WithMessage("The order item list cannot be empty.")
            .Must(items => items.Any()).WithMessage("The order must contain at least one order item");

        RuleForEach(dto => dto.OrderItems)
            .SetValidator(orderItemValidator);
    }
}
