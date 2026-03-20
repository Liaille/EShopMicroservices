using FluentValidation;

namespace Order.Application.Commands.OrderAggregate.UpdateOrderStatus;

public class UpdateOrderStatusInputDtoValidator : AbstractValidator<UpdateOrderStatusInputDto>
{
    public UpdateOrderStatusInputDtoValidator()
    {
        RuleFor(dto => dto.OrderId)
            .NotEqual(Guid.Empty).WithMessage("The order ID cannot be empty.");

        RuleFor(dto => dto.TargetStatus)
            .IsInEnum().WithMessage("The order status value is invalid.")
            .NotEqual(OrderStatus.Submitted).WithMessage("Not allowed to transfer to submitted status.");

        RuleFor(dto => dto.PaymentRecordId)
            .NotEmpty().WithMessage("The payment record ID cannot be empty.")
            .When(dto => dto.TargetStatus is OrderStatus.Paid);
    }
}
