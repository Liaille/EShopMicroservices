using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator(UpdateOrderInputDtoValidator inputDtoValidator)
    {
        RuleFor(command => command.Input)
            .NotNull().WithMessage("The input parameters for updating an order cannot be empty.")
            .SetValidator(inputDtoValidator);
    }
}
