using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.RoutingAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Routings;

/// <summary>
/// 删除工艺路线命令
/// </summary>
public record DeleteRoutingCommand(RoutingId RoutingId) : ICommand;

/// <summary>
/// 删除工艺路线命令验证器
/// </summary>
public class DeleteRoutingCommandValidator : AbstractValidator<DeleteRoutingCommand>
{
    public DeleteRoutingCommandValidator()
    {
        RuleFor(x => x.RoutingId).NotEmpty().WithMessage("工艺路线ID不能为空");
    }
}

/// <summary>
/// 删除工艺路线命令处理器
/// </summary>
public class DeleteRoutingCommandHandler(IRoutingRepository routingRepository) : ICommandHandler<DeleteRoutingCommand>
{
    public async Task Handle(DeleteRoutingCommand request, CancellationToken cancellationToken)
    {
        var routing = await routingRepository.GetAsync(request.RoutingId, cancellationToken)
                     ?? throw new KnownException($"未找到工艺路线，RoutingId = {request.RoutingId}");

        await routingRepository.DeleteAsync(routing);
    }
}

