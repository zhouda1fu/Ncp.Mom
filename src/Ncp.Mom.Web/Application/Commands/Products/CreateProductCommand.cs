using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Infrastructure.Repositories;
using Ncp.Mom.Web.Application.Queries;

namespace Ncp.Mom.Web.Application.Commands.Products;

/// <summary>
/// 创建产品命令
/// </summary>
public record CreateProductCommand(string ProductCode, string ProductName) : ICommand<ProductId>;

/// <summary>
/// 创建产品命令验证器
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(ProductQuery productQuery)
    {
        RuleFor(x => x.ProductCode).NotEmpty().WithMessage("产品编码不能为空").MaximumLength(50).WithMessage("产品编码长度不能超过50");
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("产品名称不能为空").MaximumLength(200).WithMessage("产品名称长度不能超过200");
        RuleFor(x => x.ProductCode).MustAsync(async (code, ct) => !await productQuery.DoesProductExist(code, ct))
            .WithMessage(x => $"产品编码已存在，ProductCode={x.ProductCode}");
    }
}

/// <summary>
/// 创建产品命令处理器
/// </summary>
public class CreateProductCommandHandler(IProductRepository productRepository) : ICommandHandler<CreateProductCommand, ProductId>
{
    public async Task<ProductId> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.ProductCode, request.ProductName);
        await productRepository.AddAsync(product, cancellationToken);
        return product.Id;
    }
}

