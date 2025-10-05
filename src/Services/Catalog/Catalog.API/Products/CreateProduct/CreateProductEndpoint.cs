namespace Catalog.API.Products.CreateProduct;

/// <summary>
/// 新增产品请求
/// </summary>
public record CreateProductRequest(string Name,
                                   List<string> Category,
                                   string Description,
                                   string ImageFile,
                                   decimal Price);

/// <summary>
/// 新增产品响应
/// </summary>
public record CreateProductResponse(Guid Id);

/// <summary>
/// 应用程序API层
/// </summary>
public class CreateProductEndpoint
{
}
