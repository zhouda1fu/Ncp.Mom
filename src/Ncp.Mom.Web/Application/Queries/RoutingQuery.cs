using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;

namespace Ncp.Mom.Web.Application.Queries;

public class RoutingQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Routing> RoutingSet { get; } = applicationDbContext.Routings;

    public async Task<bool> DoesRoutingExist(string routingNumber, CancellationToken cancellationToken)
    {
        return await RoutingSet.AsNoTracking()
            .AnyAsync(r => r.RoutingNumber == routingNumber, cancellationToken);
    }
}

