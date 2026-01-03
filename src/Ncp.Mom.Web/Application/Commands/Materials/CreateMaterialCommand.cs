using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Materials;

public record CreateMaterialCommand(
    string MaterialCode,
    string MaterialName,
    string? Specification = null,
    string? Unit = null) : ICommand<MaterialId>;

public class CreateMaterialCommandValidator : AbstractValidator<CreateMaterialCommand>
{
    public CreateMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialCode)
            .NotEmpty().WithMessage("物料编码不能为空")
            .MaximumLength(50).WithMessage("物料编码长度不能超过50");
        
        RuleFor(x => x.MaterialName)
            .NotEmpty().WithMessage("物料名称不能为空")
            .MaximumLength(200).WithMessage("物料名称长度不能超过200");
    }
}

public class CreateMaterialCommandHandler(
    IMaterialRepository materialRepository) 
    : ICommandHandler<CreateMaterialCommand, MaterialId>
{
    public async Task<MaterialId> Handle(
        CreateMaterialCommand request, 
        CancellationToken cancellationToken)
    {
        var material = new Material(
            request.MaterialCode,
            request.MaterialName,
            request.Specification,
            request.Unit);
        
        await materialRepository.AddAsync(material, cancellationToken);
        return material.Id;
    }
}

