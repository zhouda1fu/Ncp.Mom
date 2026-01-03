using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.BomAggregate;
using Ncp.Mom.Domain.AggregatesModel.ProductAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Boms;

public record CreateBomCommand(
    string BomNumber,
    ProductId ProductId,
    int Version) : ICommand<BomId>;

public class CreateBomCommandValidator : AbstractValidator<CreateBomCommand>
{
    public CreateBomCommandValidator()
    {
        RuleFor(x => x.BomNumber)
            .NotEmpty().WithMessage("BOM编号不能为空")
            .MaximumLength(50).WithMessage("BOM编号长度不能超过50");
        
        RuleFor(x => x.Version)
            .GreaterThan(0).WithMessage("版本号必须大于0");
    }
}

public class CreateBomCommandHandler(
    IBomRepository bomRepository) 
    : ICommandHandler<CreateBomCommand, BomId>
{
    public async Task<BomId> Handle(
        CreateBomCommand request, 
        CancellationToken cancellationToken)
    {
        var bom = new Bom(
            request.BomNumber,
            request.ProductId,
            request.Version);
        
        await bomRepository.AddAsync(bom, cancellationToken);
        return bom.Id;
    }
}

