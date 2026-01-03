using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Routings;

/// <summary>
/// 更新工艺路线命令
/// </summary>
public record UpdateRoutingCommand(RoutingId RoutingId, string RoutingNumber, string Name) : ICommand;

/// <summary>
/// 更新工艺路线命令验证器
/// </summary>
public class UpdateRoutingCommandValidator : AbstractValidator<UpdateRoutingCommand>
{
    public UpdateRoutingCommandValidator()
    {
        RuleFor(x => x.RoutingId).NotEmpty().WithMessage("工艺路线ID不能为空");
        RuleFor(x => x.RoutingNumber).NotEmpty().WithMessage("工艺路线编码不能为空").MaximumLength(50).WithMessage("工艺路线编码长度不能超过50");
        RuleFor(x => x.Name).NotEmpty().WithMessage("工艺路线名称不能为空").MaximumLength(200).WithMessage("工艺路线名称长度不能超过200");
    }
}

/// <summary>
/// 更新工艺路线命令处理器
/// </summary>
public class UpdateRoutingCommandHandler(IRoutingRepository routingRepository) : ICommandHandler<UpdateRoutingCommand>
{
    public async Task Handle(UpdateRoutingCommand request, CancellationToken cancellationToken)
    {
        var routing = await routingRepository.GetAsync(request.RoutingId, cancellationToken)
                    ?? throw new KnownException($"未找到工艺路线，RoutingId = {request.RoutingId}");

        routing.UpdateInfo(request.RoutingNumber, request.Name);
    }
}

