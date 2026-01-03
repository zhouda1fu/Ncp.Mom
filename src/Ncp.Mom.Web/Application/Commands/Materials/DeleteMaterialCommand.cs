using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.MaterialAggregate;
using Ncp.Mom.Infrastructure.Repositories;
using Ncp.Mom.Web.Application.Queries;

namespace Ncp.Mom.Web.Application.Commands.Materials;

/// <summary>
/// 删除物料命令
/// </summary>
public record DeleteMaterialCommand(MaterialId MaterialId) : ICommand;

/// <summary>
/// 删除物料命令验证器
/// </summary>
public class DeleteMaterialCommandValidator : AbstractValidator<DeleteMaterialCommand>
{
    public DeleteMaterialCommandValidator(MaterialQuery materialQuery, BomQuery bomQuery)
    {
        RuleFor(x => x.MaterialId).NotEmpty().WithMessage("物料ID不能为空");
        RuleFor(x => x.MaterialId)
            .MustAsync(async (materialId, ct) => await materialQuery.DoesMaterialExist(materialId, ct))
            .WithMessage("物料不存在");
        RuleFor(x => x.MaterialId)
            .MustAsync(async (materialId, ct) => !await bomQuery.IsMaterialUsedInBomAsync(materialId, ct))
            .WithMessage("物料正在被 BOM 使用，无法删除");
    }
}

/// <summary>
/// 删除物料命令处理器
/// </summary>
public class DeleteMaterialCommandHandler(
    IMaterialRepository materialRepository) 
    : ICommandHandler<DeleteMaterialCommand>
{
    public async Task Handle(
        DeleteMaterialCommand request, 
        CancellationToken cancellationToken)
    {
        var material = await materialRepository.GetAsync(
            request.MaterialId, 
            cancellationToken)
            ?? throw new KnownException($"未找到物料，MaterialId = {request.MaterialId}");

        await materialRepository.DeleteAsync(material);
    }
}

