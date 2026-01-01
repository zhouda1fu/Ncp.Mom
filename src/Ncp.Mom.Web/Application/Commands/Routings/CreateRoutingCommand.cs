using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Infrastructure.Repositories;
using Ncp.Mom.Web.Application.Queries;

namespace Ncp.Mom.Web.Application.Commands.Routings;

public record CreateRoutingCommand(
    string RoutingNumber,
    string Name,
    ProductId ProductId) : ICommand<RoutingId>;

public class CreateRoutingCommandValidator : AbstractValidator<CreateRoutingCommand>
{
    public CreateRoutingCommandValidator(RoutingQuery routingQuery)
    {
        RuleFor(x => x.RoutingNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.RoutingNumber)
            .MustAsync(async (routingNumber, ct) => !await routingQuery.DoesRoutingExist(routingNumber, ct))
            .WithMessage(x => $"工艺路线编号 {x.RoutingNumber} 已存在");
    }
}

public class CreateRoutingCommandHandler(
    IRoutingRepository routingRepository,
    ILogger<CreateRoutingCommandHandler> logger)
    : ICommandHandler<CreateRoutingCommand, RoutingId>
{
    public async Task<RoutingId> Handle(
        CreateRoutingCommand request,
        CancellationToken cancellationToken)
    {
        var routing = new Routing(
            request.RoutingNumber,
            request.Name,
            request.ProductId);

        routing = await routingRepository.AddAsync(routing, cancellationToken);
        logger.LogInformation("工艺路线已创建，ID: {RoutingId}, 工艺路线编号: {RoutingNumber}",
            routing.Id, routing.RoutingNumber);
        return routing.Id;
    }
}

