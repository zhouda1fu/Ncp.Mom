using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Infrastructure.Repositories;
using Ncp.Mom.Web.Application.Queries;

namespace Ncp.Mom.Web.Application.Commands.WorkCenters;

/// <summary>
/// 创建工作中心命令
/// </summary>
public record CreateWorkCenterCommand(string WorkCenterCode, string WorkCenterName) : ICommand<WorkCenterId>;

/// <summary>
/// 创建工作中心命令验证器
/// </summary>
public class CreateWorkCenterCommandValidator : AbstractValidator<CreateWorkCenterCommand>
{
    public CreateWorkCenterCommandValidator(WorkCenterQuery workCenterQuery)
    {
        RuleFor(x => x.WorkCenterCode).NotEmpty().WithMessage("工作中心编码不能为空").MaximumLength(50).WithMessage("工作中心编码长度不能超过50");
        RuleFor(x => x.WorkCenterName).NotEmpty().WithMessage("工作中心名称不能为空").MaximumLength(200).WithMessage("工作中心名称长度不能超过200");
        RuleFor(x => x.WorkCenterCode).MustAsync(async (code, ct) => !await workCenterQuery.DoesWorkCenterExist(code, ct))
            .WithMessage(x => $"工作中心编码已存在，WorkCenterCode={x.WorkCenterCode}");
    }
}

/// <summary>
/// 创建工作中心命令处理器
/// </summary>
public class CreateWorkCenterCommandHandler(IWorkCenterRepository workCenterRepository) : ICommandHandler<CreateWorkCenterCommand, WorkCenterId>
{
    public async Task<WorkCenterId> Handle(CreateWorkCenterCommand request, CancellationToken cancellationToken)
    {
        var workCenter = new WorkCenter(request.WorkCenterCode, request.WorkCenterName);
        await workCenterRepository.AddAsync(workCenter, cancellationToken);
        return workCenter.Id;
    }
}

