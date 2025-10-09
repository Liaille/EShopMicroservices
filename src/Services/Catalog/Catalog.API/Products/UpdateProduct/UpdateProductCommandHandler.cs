
namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id,
                                   string Name,
                                   List<string> Categories,
                                   string Description,
                                   string ImageFile,
                                   decimal Price) : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

internal class UpdateProductCommandHandler(IDocumentSession session,
                                           ILogger<UpdateProductCommandHandler> logger) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("{ClassName}.{MethodName} call with {Command}", GetType().FullName, nameof(Handle), command);

        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
        
        if (product is null)
        {
            logger.LogWarning("Product with Id {ProductId} not found.", command.Id);
            throw new ProductNotFoundException($"Product with Id {command.Id} not found.");
        }

        product.Name = command.Name;
        product.Categories = command.Categories;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}
