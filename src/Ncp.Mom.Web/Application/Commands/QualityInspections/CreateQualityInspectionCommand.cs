using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkOrderAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.QualityInspections;

/// <summary>
/// 创建质检单命令
/// </summary>
public record CreateQualityInspectionCommand(
    string InspectionNumber,
    WorkOrderId WorkOrderId,
    int SampleQuantity) : ICommand<QualityInspectionId>;

/// <summary>
/// 创建质检单命令验证器
/// </summary>
public class CreateQualityInspectionCommandValidator : AbstractValidator<CreateQualityInspectionCommand>
{
    public CreateQualityInspectionCommandValidator(IWorkOrderRepository workOrderRepository)
    {
        RuleFor(x => x.InspectionNumber)
            .NotEmpty().WithMessage("质检单编号不能为空")
            .MaximumLength(50).WithMessage("质检单编号长度不能超过50");
        
        RuleFor(x => x.SampleQuantity)
            .GreaterThan(0).WithMessage("抽样数量必须大于0");
        
        RuleFor(x => x.WorkOrderId)
            .MustAsync(async (workOrderId, ct) => 
            {
                var workOrder = await workOrderRepository.GetAsync(workOrderId, ct);
                return workOrder != null;
            })
            .WithMessage("工单不存在");
    }
}

/// <summary>
/// 创建质检单命令处理器
/// </summary>
public class CreateQualityInspectionCommandHandler(
    IQualityInspectionRepository qualityInspectionRepository) 
    : ICommandHandler<CreateQualityInspectionCommand, QualityInspectionId>
{
    public async Task<QualityInspectionId> Handle(
        CreateQualityInspectionCommand request, 
        CancellationToken cancellationToken)
    {
        var qualityInspection = new QualityInspection(
            request.InspectionNumber,
            request.WorkOrderId,
            request.SampleQuantity);
        
        await qualityInspectionRepository.AddAsync(qualityInspection, cancellationToken);
        return qualityInspection.Id;
    }
}

