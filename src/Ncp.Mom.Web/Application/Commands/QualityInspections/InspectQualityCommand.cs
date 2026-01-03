using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.QualityInspectionAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.QualityInspections;

/// <summary>
/// 执行质检命令
/// </summary>
public record InspectQualityCommand(
    QualityInspectionId QualityInspectionId,
    int QualifiedQuantity,
    int UnqualifiedQuantity,
    string? Remark) : ICommand;

/// <summary>
/// 执行质检命令验证器
/// </summary>
public class InspectQualityCommandValidator : AbstractValidator<InspectQualityCommand>
{
    public InspectQualityCommandValidator()
    {
        RuleFor(x => x.QualifiedQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("合格数量不能为负数");
        
        RuleFor(x => x.UnqualifiedQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("不合格数量不能为负数");
        
        RuleFor(x => x)
            .Must(x => x.QualifiedQuantity + x.UnqualifiedQuantity > 0)
            .WithMessage("合格数量和不合格数量之和必须大于0");
    }
}

/// <summary>
/// 执行质检命令处理器
/// </summary>
public class InspectQualityCommandHandler(
    IQualityInspectionRepository qualityInspectionRepository) 
    : ICommandHandler<InspectQualityCommand>
{
    public async Task Handle(
        InspectQualityCommand request, 
        CancellationToken cancellationToken)
    {
        var qualityInspection = await qualityInspectionRepository.GetAsync(
            request.QualityInspectionId, 
            cancellationToken)
            ?? throw new KnownException("质检单不存在");

        qualityInspection.Inspect(
            request.QualifiedQuantity,
            request.UnqualifiedQuantity,
            request.Remark);
    }
}

