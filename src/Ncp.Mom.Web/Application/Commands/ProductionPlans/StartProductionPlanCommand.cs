using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.ProductionPlans;

public record StartProductionPlanCommand(ProductionPlanId Id) : ICommand;

public class StartProductionPlanCommandValidator : AbstractValidator<StartProductionPlanCommand>
{
    public StartProductionPlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class StartProductionPlanCommandHandler(
    IProductionPlanRepository productionPlanRepository,
    ILogger<StartProductionPlanCommandHandler> logger)
    : ICommandHandler<StartProductionPlanCommand>
{
    public async Task Handle(
        StartProductionPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = await productionPlanRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new KnownException("生产计划不存在");

        plan.Start();
        logger.LogInformation("生产计划已启动，ID: {ProductionPlanId}", request.Id);
    }
}

