using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.CancelOrder;

public class CancelOrderInputDtoValidator : AbstractValidator<CancelOrderInputDto>
{
    public CancelOrderInputDtoValidator()
    {
        RuleFor(dto => dto.OrderId)
            .NotEqual(Guid.Empty).WithMessage("The order ID cannot be empty.");

        RuleFor(dto => dto.CustomerId)
            .NotEqual(Guid.Empty).WithMessage("The customer ID cannot be empty.");

        RuleFor(dto => dto.CancelReason)
            .MaximumLength(500).WithMessage("The reason for cancellation cannot exceed 500 characters in length.")
            .When(dto => !string.IsNullOrWhiteSpace(dto.CancelReason));
    }
}
