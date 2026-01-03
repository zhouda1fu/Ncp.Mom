using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.ProductionPlans;

/// <summary>
/// 完成生产计划命令
/// </summary>
public record CompleteProductionPlanCommand(ProductionPlanId ProductionPlanId) : ICommand;

/// <summary>
/// 完成生产计划命令验证器
/// </summary>
public class CompleteProductionPlanCommandValidator : AbstractValidator<CompleteProductionPlanCommand>
{
    public CompleteProductionPlanCommandValidator()
    {
        RuleFor(x => x.ProductionPlanId).NotEmpty().WithMessage("生产计划ID不能为空");
    }
}

/// <summary>
/// 完成生产计划命令处理器
/// </summary>
public class CompleteProductionPlanCommandHandler(IProductionPlanRepository productionPlanRepository) : ICommandHandler<CompleteProductionPlanCommand>
{
    public async Task Handle(CompleteProductionPlanCommand request, CancellationToken cancellationToken)
    {
        var productionPlan = await productionPlanRepository.GetAsync(request.ProductionPlanId, cancellationToken)
                            ?? throw new KnownException($"未找到生产计划，ProductionPlanId = {request.ProductionPlanId}");

        productionPlan.Complete();
    }
}

