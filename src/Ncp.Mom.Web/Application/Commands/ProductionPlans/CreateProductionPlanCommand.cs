using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Infrastructure.Repositories;
using Ncp.Mom.Web.Application.Queries;

namespace Ncp.Mom.Web.Application.Commands.ProductionPlans;

public record CreateProductionPlanCommand(
    string PlanNumber,
    ProductId ProductId,
    int Quantity,
    DateTime PlannedStartDate,
    DateTime PlannedEndDate) : ICommand<ProductionPlanId>;

public class CreateProductionPlanCommandValidator : AbstractValidator<CreateProductionPlanCommand>
{
    public CreateProductionPlanCommandValidator(ProductionPlanQuery productionPlanQuery)
    {
        RuleFor(x => x.PlanNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.PlannedEndDate).GreaterThan(x => x.PlannedStartDate)
            .WithMessage("计划结束日期必须大于计划开始日期");
        RuleFor(x => x.PlanNumber)
            .MustAsync(async (planNumber, ct) => !await productionPlanQuery.DoesProductionPlanExist(planNumber, ct))
            .WithMessage(x => $"生产计划编号 {x.PlanNumber} 已存在");
    }
}

public class CreateProductionPlanCommandHandler(
    IProductionPlanRepository productionPlanRepository,
    ILogger<CreateProductionPlanCommandHandler> logger)
    : ICommandHandler<CreateProductionPlanCommand, ProductionPlanId>
{
    public async Task<ProductionPlanId> Handle(
        CreateProductionPlanCommand request,
        CancellationToken cancellationToken)
    {
        var plan = new ProductionPlan(
            request.PlanNumber,
            request.ProductId,
            request.Quantity,
            request.PlannedStartDate,
            request.PlannedEndDate);

        plan = await productionPlanRepository.AddAsync(plan, cancellationToken);
        logger.LogInformation("生产计划已创建，ID: {ProductionPlanId}, 计划编号: {PlanNumber}",
            plan.Id, plan.PlanNumber);
        return plan.Id;
    }
}

