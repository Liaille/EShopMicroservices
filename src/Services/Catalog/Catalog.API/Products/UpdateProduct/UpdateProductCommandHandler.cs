
namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id,
                                   string Name,
                                   List<string> Categories,
                                   string Description,
                                   string ImageFile,
                                   decimal Price) : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("产品Id不能为空");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("产品名称不能为空")
            .Length(2,100).WithMessage("产品名称不能少于2个字符或超过100个字符");
        RuleFor(x => x.Categories)
            .NotEmpty().WithMessage("产品类别不能为空")
            .Must(categories => categories.All(c => !string.IsNullOrWhiteSpace(c)))
            .WithMessage("产品类别列表中不能包含空类别");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("产品描述不能为空")
            .MaximumLength(1000).WithMessage("产品描述不能超过1000个字符");
        RuleFor(x => x.ImageFile)
            .NotEmpty().WithMessage("产品图片文件不能为空")
            .MaximumLength(200).WithMessage("产品图片文件路径不能超过200个字符");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("产品价格必须大于0");
    }
}

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
            throw new ProductNotFoundException(command.Id);
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
