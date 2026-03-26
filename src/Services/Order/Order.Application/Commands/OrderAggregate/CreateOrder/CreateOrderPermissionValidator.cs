namespace Order.Application.Commands.OrderAggregate.CreateOrder;

public sealed class CreateOrderPermissionValidator : IPermissionValidator<CreateOrderCommand>
{
    public async Task ValidateAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        if (command.Input.CustomerId == Guid.Empty) throw new UnauthorizedAccessException("Unauthenticated users cannot create orders.");

        await Task.CompletedTask;
    }
}
