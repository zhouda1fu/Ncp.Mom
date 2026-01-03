using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.WorkCenters;

/// <summary>
/// 更新工作中心命令
/// </summary>
public record UpdateWorkCenterCommand(WorkCenterId WorkCenterId, string WorkCenterCode, string WorkCenterName) : ICommand;

/// <summary>
/// 更新工作中心命令验证器
/// </summary>
public class UpdateWorkCenterCommandValidator : AbstractValidator<UpdateWorkCenterCommand>
{
    public UpdateWorkCenterCommandValidator()
    {
        RuleFor(x => x.WorkCenterId).NotEmpty().WithMessage("工作中心ID不能为空");
        RuleFor(x => x.WorkCenterCode).NotEmpty().WithMessage("工作中心编码不能为空").MaximumLength(50).WithMessage("工作中心编码长度不能超过50");
        RuleFor(x => x.WorkCenterName).NotEmpty().WithMessage("工作中心名称不能为空").MaximumLength(200).WithMessage("工作中心名称长度不能超过200");
    }
}

/// <summary>
/// 更新工作中心命令处理器
/// </summary>
public class UpdateWorkCenterCommandHandler(IWorkCenterRepository workCenterRepository) : ICommandHandler<UpdateWorkCenterCommand>
{
    public async Task Handle(UpdateWorkCenterCommand request, CancellationToken cancellationToken)
    {
        var workCenter = await workCenterRepository.GetAsync(request.WorkCenterId, cancellationToken)
                        ?? throw new KnownException($"未找到工作中心，WorkCenterId = {request.WorkCenterId}");

        workCenter.UpdateInfo(request.WorkCenterCode, request.WorkCenterName);
    }
}

