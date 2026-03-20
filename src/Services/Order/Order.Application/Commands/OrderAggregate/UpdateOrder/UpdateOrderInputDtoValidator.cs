using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.UpdateOrder;

public class UpdateOrderInputDtoValidator : AbstractValidator<UpdateOrderInputDto>
{
    public UpdateOrderInputDtoValidator(AddressDtoValidator addressValidator)
    {
        RuleFor(dto => dto.OrderId)
            .NotEqual(Guid.Empty).WithMessage("The order ID cannot be empty.");

        RuleFor(dto => dto.CustomerId)
            .NotEqual(Guid.Empty).WithMessage("The customer ID cannot be empty.");

        RuleFor(dto => dto.OrderName)
            .NotEmpty().WithMessage("Order name cannot be empty.")
            .MaximumLength(100).WithMessage("The length of the order name cannot exceed 100 characters.")
            .Matches(@"^[^<>/\\|""*?]+$").WithMessage("The order name cannot contain special characters (<>/\\ \\ | \"\" *?)");

        RuleFor(dto => dto.ShippingAddress)
            .NotNull().WithMessage("Shipping address cannot be empty.")
            .SetValidator(addressValidator);

        RuleFor(dto => dto.BillingAddress)
            .NotNull().WithMessage("Billing address cannot be empty.")
            .SetValidator(addressValidator);

        RuleFor(dto => dto.PaymentMethodId)
            .NotEqual(Guid.Empty).WithMessage("Payment method ID cannot be empty.");
    }
}
