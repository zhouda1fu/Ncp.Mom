using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Products;

/// <summary>
/// 更新产品命令
/// </summary>
public record UpdateProductCommand(ProductId ProductId, string ProductCode, string ProductName) : ICommand;

/// <summary>
/// 更新产品命令验证器
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("产品ID不能为空");
        RuleFor(x => x.ProductCode).NotEmpty().WithMessage("产品编码不能为空").MaximumLength(50).WithMessage("产品编码长度不能超过50");
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("产品名称不能为空").MaximumLength(200).WithMessage("产品名称长度不能超过200");
    }
}

/// <summary>
/// 更新产品命令处理器
/// </summary>
public class UpdateProductCommandHandler(IProductRepository productRepository) : ICommandHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetAsync(request.ProductId, cancellationToken)
                     ?? throw new KnownException($"未找到产品，ProductId = {request.ProductId}");

        product.UpdateInfo(request.ProductCode, request.ProductName);
    }
}

