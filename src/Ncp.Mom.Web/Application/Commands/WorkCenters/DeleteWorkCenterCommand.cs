using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.WorkCenters;

/// <summary>
/// 删除工作中心命令
/// </summary>
public record DeleteWorkCenterCommand(WorkCenterId WorkCenterId) : ICommand;

/// <summary>
/// 删除工作中心命令验证器
/// </summary>
public class DeleteWorkCenterCommandValidator : AbstractValidator<DeleteWorkCenterCommand>
{
    public DeleteWorkCenterCommandValidator()
    {
        RuleFor(x => x.WorkCenterId).NotEmpty().WithMessage("工作中心ID不能为空");
    }
}

/// <summary>
/// 删除工作中心命令处理器
/// </summary>
public class DeleteWorkCenterCommandHandler(IWorkCenterRepository workCenterRepository) : ICommandHandler<DeleteWorkCenterCommand>
{
    public async Task Handle(DeleteWorkCenterCommand request, CancellationToken cancellationToken)
    {
        var workCenter = await workCenterRepository.GetAsync(request.WorkCenterId, cancellationToken)
                        ?? throw new KnownException($"未找到工作中心，WorkCenterId = {request.WorkCenterId}");

        await workCenterRepository.DeleteAsync(workCenter);
    }
}

