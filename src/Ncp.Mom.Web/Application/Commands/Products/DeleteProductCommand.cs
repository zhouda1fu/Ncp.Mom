using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Products;

/// <summary>
/// 删除产品命令
/// </summary>
public record DeleteProductCommand(ProductId ProductId) : ICommand;

/// <summary>
/// 删除产品命令验证器
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("产品ID不能为空");
    }
}

/// <summary>
/// 删除产品命令处理器
/// </summary>
public class DeleteProductCommandHandler(IProductRepository productRepository) : ICommandHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetAsync(request.ProductId, cancellationToken)
                     ?? throw new KnownException($"未找到产品，ProductId = {request.ProductId}");

        await productRepository.DeleteAsync(product);
    }
}

