using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.ProductionPlans;

public record ApproveProductionPlanCommand(ProductionPlanId Id) : ICommand;

public class ApproveProductionPlanCommandValidator : AbstractValidator<ApproveProductionPlanCommand>
{
    public ApproveProductionPlanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class ApproveProductionPlanCommandHandler(
    IProductionPlanRepository productionPlanRepository,
    ILogger<ApproveProductionPlanCommandHandler> logger)
    : ICommandHandler<ApproveProductionPlanCommand>
{
    public async Task Handle(
        ApproveProductionPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = await productionPlanRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new KnownException("生产计划不存在");

        plan.Approve();
        logger.LogInformation("生产计划已审批，ID: {ProductionPlanId}", request.Id);
    }
}

