using BuildingBlocks.Validations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Behaviors;

public class BusinessValidationBehavior<TRequest, TResponse>(IServiceProvider serviceProvider)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validator = serviceProvider.GetService<IBusinessValidator<TRequest>>();
        if (validator != null)
            await validator.ValidateAsync(request, cancellationToken);

        return await next(cancellationToken);
    }
}
