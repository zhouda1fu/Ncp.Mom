using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

namespace Ncp.Mom.Web.Application.Queries;

/// <summary>
/// 产品查询DTO
/// </summary>
public record ProductQueryDto(ProductId Id, string ProductCode, string ProductName);

/// <summary>
/// 产品查询输入
/// </summary>
public class ProductQueryInput : PageRequest
{
    public string? Keyword { get; set; }
}

/// <summary>
/// 产品查询服务
/// </summary>
public class ProductQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Product> ProductSet { get; } = applicationDbContext.Products;

    /// <summary>
    /// 检查产品编码是否存在
    /// </summary>
    public async Task<bool> DoesProductExist(string productCode, CancellationToken cancellationToken)
    {
        return await ProductSet.AsNoTracking()
            .AnyAsync(p => p.ProductCode == productCode, cancellationToken);
    }

    /// <summary>
    /// 检查产品是否存在
    /// </summary>
    public async Task<bool> DoesProductExist(ProductId productId, CancellationToken cancellationToken)
    {
        return await ProductSet.AsNoTracking()
            .AnyAsync(p => p.Id == productId, cancellationToken);
    }

    /// <summary>
    /// 根据ID获取产品
    /// </summary>
    public async Task<ProductQueryDto?> GetProductByIdAsync(ProductId productId, CancellationToken cancellationToken)
    {
        return await ProductSet.AsNoTracking()
            .Where(p => p.Id == productId)
            .Select(p => new ProductQueryDto(p.Id, p.ProductCode, p.ProductName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 获取产品列表（分页）
    /// </summary>
    public async Task<PagedData<ProductQueryDto>> GetProductsAsync(ProductQueryInput query, CancellationToken cancellationToken)
    {
        var queryable = ProductSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(p => p.ProductCode.Contains(query.Keyword) || p.ProductName.Contains(query.Keyword));
        }

        return await queryable
            .OrderBy(p => p.ProductCode)
            .Select(p => new ProductQueryDto(p.Id, p.ProductCode, p.ProductName))
            .ToPagedDataAsync(query, cancellationToken);
    }
}

