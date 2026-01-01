using Microsoft.EntityFrameworkCore;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;

namespace Ncp.Mom.Web.Application.Queries.Routings;

public record GetRoutingQuery(RoutingId Id) : IQuery<RoutingDto>;

public record RoutingOperationDto(
    int Sequence,
    string OperationName,
    WorkCenterId WorkCenterId,
    decimal StandardTime);

public record RoutingDto(
    RoutingId Id,
    string RoutingNumber,
    string Name,
    ProductId ProductId,
    List<RoutingOperationDto> Operations);

public class GetRoutingQueryHandler(ApplicationDbContext applicationDbContext)
    : IQueryHandler<GetRoutingQuery, RoutingDto>
{
    public async Task<RoutingDto> Handle(
        GetRoutingQuery request,
        CancellationToken cancellationToken)
    {
        var routing = await applicationDbContext.Set<Routing>()
            .AsNoTracking()
            .Where(r => r.Id == request.Id)
            .Select(r => new RoutingDto(
                r.Id,
                r.RoutingNumber,
                r.Name,
                r.ProductId,
                r.Operations
                    .OrderBy(o => o.Sequence)
                    .Select(o => new RoutingOperationDto(
                        o.Sequence,
                        o.OperationName,
                        o.WorkCenterId,
                        o.StandardTime))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (routing == null)
        {
            throw new KnownException($"未找到工艺路线，Id = {request.Id}");
        }

        return routing;
    }
}

