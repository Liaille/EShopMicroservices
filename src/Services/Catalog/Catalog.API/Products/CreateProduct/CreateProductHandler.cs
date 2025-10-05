using BuildingBlocks.CQRS;
using Catalog.API.Models;

namespace Catalog.API.Products.CreateProduct;

/// <summary>
/// 新增产品命令
/// </summary>
public record CreateProductCommand(string Name,
                                   List<string> Category,
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
public class CreateProductHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // 从命令对象创建产品实体
        Product product = new()
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };
        // 将产品实体保存到数据库

        // 返回创建的产品ID
        return Task.FromResult(new CreateProductResult(product.Id));
    }
}
