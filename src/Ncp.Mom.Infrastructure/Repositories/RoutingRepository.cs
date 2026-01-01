using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;

namespace Ncp.Mom.Infrastructure.Repositories;

public interface IRoutingRepository : IRepository<Routing, RoutingId>
{
    Task<List<Routing>> GetByProductIdAsync(ProductId productId, CancellationToken cancellationToken = default);
}

public class RoutingRepository(ApplicationDbContext context)
    : RepositoryBase<Routing, RoutingId, ApplicationDbContext>(context),
      IRoutingRepository
{
    public async Task<List<Routing>> GetByProductIdAsync(
        ProductId productId,
        CancellationToken cancellationToken = default)
    {
        return await context.Set<Routing>()
            .Include(r => r.Operations)
            .Where(r => r.ProductId == productId)
            .ToListAsync(cancellationToken);
    }
}

