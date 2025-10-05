using MediatR;

namespace Catalog.API.Products.CreateProduct;

/// <summary>
/// 新增产品请求模型
/// </summary>
public record CreateProductCommand(string Name,
                                   List<string> Category,
                                   string Description,
                                   string ImageFile,
                                   decimal Price) : IRequest<CreateProductResult>;

/// <summary>
/// 新增产品响应模型
/// </summary>
public record CreateProductResult(Guid Id);

/// <summary>
/// 应用程序逻辑层
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    public Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
