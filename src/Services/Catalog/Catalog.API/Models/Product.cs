namespace Catalog.API.Models;

/// <summary>
/// 产品
/// </summary>
public class Product
{
    /// <summary>
    /// 主键
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 类别列表
    /// </summary>
    public List<string> Category { get; set; } = [];

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 图片文件
    /// </summary>
    public string ImageFile { get; set; } = default!;

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }
}
