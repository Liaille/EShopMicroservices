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

/// <summary>
/// 应用程序逻辑层
/// </summary>
internal class CreateProductCommandHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
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
