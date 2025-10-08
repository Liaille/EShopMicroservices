namespace Catalog.API.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

public record GetProductByIdResult(Product Product);

internal class GetProductByIdQueryHandler(
    IDocumentSession session,
    ILogger<GetProductByIdQueryHandler> logger) : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("{ClassName}.{MethodName} call with {Query}", GetType().FullName, nameof(Handle), query);

        Product? product = await session.LoadAsync<Product>(query.Id, cancellationToken);

        if (product is null)
        {
            logger.LogWarning("Product with Id {ProductId} not found.", query.Id);
            throw new KeyNotFoundException($"Product with Id {query.Id} not found.");
        }

        return new GetProductByIdResult(product);
    }
}
