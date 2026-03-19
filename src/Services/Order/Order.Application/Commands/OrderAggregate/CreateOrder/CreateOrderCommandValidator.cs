using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(CreateOrderInputDtoValidator inputDtoValidator)
    {
        RuleFor(command => command.Input)
            .NotNull().WithMessage("The input parameters for creating an order cannot be empty.")
            .SetValidator(inputDtoValidator);
    }
}
