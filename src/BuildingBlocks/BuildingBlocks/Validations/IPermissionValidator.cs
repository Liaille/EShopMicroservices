namespace BuildingBlocks.Validations;

public interface IPermissionValidator<TRequest>
{
    Task ValidateAsync(TRequest request, CancellationToken cancellationToken);
}
