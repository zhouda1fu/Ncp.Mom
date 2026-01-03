using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

/// <summary>
/// 产品仓储接口
/// </summary>
public interface IProductRepository : IRepository<Product, ProductId> { }

/// <summary>
/// 产品仓储实现
/// </summary>
public class ProductRepository(ApplicationDbContext context) : RepositoryBase<Product, ProductId, ApplicationDbContext>(context), IProductRepository { }

