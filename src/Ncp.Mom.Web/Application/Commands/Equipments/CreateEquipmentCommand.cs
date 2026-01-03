using FluentValidation;
using Ncp.Mom.Domain.AggregatesModel.EquipmentAggregate;
using Ncp.Mom.Domain.AggregatesModel.WorkCenterAggregate;
using Ncp.Mom.Infrastructure.Repositories;

namespace Ncp.Mom.Web.Application.Commands.Equipments;

public record CreateEquipmentCommand(
    string EquipmentCode,
    string EquipmentName,
    EquipmentType EquipmentType,
    WorkCenterId? WorkCenterId) : ICommand<EquipmentId>;

public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
    public CreateEquipmentCommandValidator()
    {
        RuleFor(x => x.EquipmentCode)
            .NotEmpty().WithMessage("设备编码不能为空")
            .MaximumLength(50).WithMessage("设备编码长度不能超过50");
        
        RuleFor(x => x.EquipmentName)
            .NotEmpty().WithMessage("设备名称不能为空")
            .MaximumLength(200).WithMessage("设备名称长度不能超过200");
    }
}

public class CreateEquipmentCommandHandler(
    IEquipmentRepository equipmentRepository) 
    : ICommandHandler<CreateEquipmentCommand, EquipmentId>
{
    public async Task<EquipmentId> Handle(
        CreateEquipmentCommand request, 
        CancellationToken cancellationToken)
    {
        var equipment = new Equipment(
            request.EquipmentCode,
            request.EquipmentName,
            request.EquipmentType,
            request.WorkCenterId);
        
        await equipmentRepository.AddAsync(equipment, cancellationToken);
        return equipment.Id;
    }
}

