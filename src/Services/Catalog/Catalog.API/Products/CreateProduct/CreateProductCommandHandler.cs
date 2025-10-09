namespace Catalog.API.Products.CreateProduct;

/// <summary>
/// 新增产品命令
/// </summary>
public record CreateProductCommand(string Name,
                                   List<string> Categories,
                                   string Description,
                                   string ImageFile,
                                   decimal Price) : ICommand<CreateProductResult>;

/// <summary>
/// 新增产品结果
/// </summary>
public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("产品名称不能为空")
            .MaximumLength(100).WithMessage("产品名称不能超过100个字符");
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

/// <summary>
/// 应用程序逻辑层
/// </summary>
internal class CreateProductCommandHandler(IDocumentSession session,
                                           ILogger<CreateProductCommandHandler> logger) : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("{ClassName}.{MethodName} call with {Command}", GetType().FullName, nameof(Handle), command);

        #region 从命令对象创建产品实体
        Product product = new()
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Categories = command.Categories,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };
        #endregion

        #region 将产品实体保存到数据库
        // 因UseLightweightSessions缺少自动脏检查机制，需手动调用Store方法维护数据一致性
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);
        #endregion

        // 返回创建的产品ID
        return new CreateProductResult(product.Id);
    }
}
