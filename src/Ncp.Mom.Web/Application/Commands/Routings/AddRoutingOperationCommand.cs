using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Routings;

public record AddRoutingOperationCommand(
    RoutingId RoutingId,
    int Sequence,
    string OperationName,
    WorkCenterId WorkCenterId,
    decimal StandardTime) : ICommand;

public class AddRoutingOperationCommandValidator : AbstractValidator<AddRoutingOperationCommand>
{
    public AddRoutingOperationCommandValidator()
    {
        RuleFor(x => x.RoutingId).NotEmpty();
        RuleFor(x => x.Sequence).GreaterThan(0);
        RuleFor(x => x.OperationName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WorkCenterId).NotEmpty();
        RuleFor(x => x.StandardTime).GreaterThan(0);
    }
}

public class AddRoutingOperationCommandHandler(
    IRoutingRepository routingRepository,
    ILogger<AddRoutingOperationCommandHandler> logger)
    : ICommandHandler<AddRoutingOperationCommand>
{
    public async Task Handle(
        AddRoutingOperationCommand request,
        CancellationToken cancellationToken)
    {
        var routing = await routingRepository.GetAsync(request.RoutingId, cancellationToken)
            ?? throw new KnownException("工艺路线不存在");

        routing.AddOperation(
            request.Sequence,
            request.OperationName,
            request.WorkCenterId,
            request.StandardTime);

        logger.LogInformation("工艺路线 {RoutingId} 已添加工序，序号: {Sequence}",
            request.RoutingId, request.Sequence);
    }
}

