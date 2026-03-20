using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator(UpdateOrderStatusInputDtoValidator inputDtoValidator)
    {
        RuleFor(dto => dto.Input)
            .NotNull().WithMessage("The input parameters for updating an order status cannot be empty.")
            .SetValidator(inputDtoValidator);
    }
}
