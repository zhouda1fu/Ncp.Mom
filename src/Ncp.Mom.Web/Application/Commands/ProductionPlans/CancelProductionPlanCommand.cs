using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.ProductionPlanAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.ProductionPlans;

/// <summary>
/// 取消生产计划命令
/// </summary>
public record CancelProductionPlanCommand(ProductionPlanId ProductionPlanId) : ICommand;

/// <summary>
/// 取消生产计划命令验证器
/// </summary>
public class CancelProductionPlanCommandValidator : AbstractValidator<CancelProductionPlanCommand>
{
    public CancelProductionPlanCommandValidator()
    {
        RuleFor(x => x.ProductionPlanId).NotEmpty().WithMessage("生产计划ID不能为空");
    }
}

/// <summary>
/// 取消生产计划命令处理器
/// </summary>
public class CancelProductionPlanCommandHandler(IProductionPlanRepository productionPlanRepository) : ICommandHandler<CancelProductionPlanCommand>
{
    public async Task Handle(CancelProductionPlanCommand request, CancellationToken cancellationToken)
    {
        var productionPlan = await productionPlanRepository.GetAsync(request.ProductionPlanId, cancellationToken)
                            ?? throw new KnownException($"未找到生产计划，ProductionPlanId = {request.ProductionPlanId}");

        productionPlan.Cancel();
    }
}

