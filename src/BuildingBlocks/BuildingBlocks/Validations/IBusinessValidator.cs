namespace BuildingBlocks.Validations;

public interface IBusinessValidator<TRequest>
{
    Task ValidateAsync(TRequest request, CancellationToken cancellationToken);
}
