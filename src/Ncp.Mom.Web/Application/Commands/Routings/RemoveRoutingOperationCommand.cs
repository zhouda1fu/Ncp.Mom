using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Routings;

/// <summary>
/// 删除工艺路线工序命令
/// </summary>
public record RemoveRoutingOperationCommand(RoutingId RoutingId, int Sequence) : ICommand;

/// <summary>
/// 删除工艺路线工序命令验证器
/// </summary>
public class RemoveRoutingOperationCommandValidator : AbstractValidator<RemoveRoutingOperationCommand>
{
    public RemoveRoutingOperationCommandValidator()
    {
        RuleFor(x => x.RoutingId).NotEmpty().WithMessage("工艺路线ID不能为空");
        RuleFor(x => x.Sequence).GreaterThan(0).WithMessage("工序序号必须大于0");
    }
}

/// <summary>
/// 删除工艺路线工序命令处理器
/// </summary>
public class RemoveRoutingOperationCommandHandler(IRoutingRepository routingRepository) : ICommandHandler<RemoveRoutingOperationCommand>
{
    public async Task Handle(RemoveRoutingOperationCommand request, CancellationToken cancellationToken)
    {
        var routing = await routingRepository.GetAsync(request.RoutingId, cancellationToken)
                     ?? throw new KnownException($"未找到工艺路线，RoutingId = {request.RoutingId}");

        routing.RemoveOperation(request.Sequence);
    }
}

