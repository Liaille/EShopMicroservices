namespace Catalog.API.Products.CreateProduct;

/// <summary>
/// 新增产品请求
/// </summary>
public record CreateProductRequest(string Name,
                                   List<string> Categories,
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
public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", async (CreateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateProductCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateProductResponse>();
            return Results.Created($"/products/{response.Id}", response);
        })
        .WithName("CreateProduct") // 指定路由名称
        .Produces<CreateProductResponse>(StatusCodes.Status201Created) // 指定成功响应类型和状态码
        .ProducesProblem(StatusCodes.Status400BadRequest) // 指定可能的错误响应
        .WithSummary("Creates a new product") // 添加摘要
        .WithDescription("Creates a new product with the provided details."); // 添加描述
    }
}
