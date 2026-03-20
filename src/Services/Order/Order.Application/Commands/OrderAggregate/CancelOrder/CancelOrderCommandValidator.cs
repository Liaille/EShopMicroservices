using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.CancelOrder;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator(CancelOrderInputDtoValidator inputDtoValidator)
    {
        RuleFor(dto => dto.Input)
            .NotNull().WithMessage("The input parameter cannot be empty.")
            .SetValidator(inputDtoValidator);
    }
}
