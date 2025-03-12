using FluentValidation;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.Extensions;
using PetFamily.Core.SharedVO;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdateTransferDetails;

public class UpdateTransferDetailsCommandValidator : AbstractValidator<UpdateTransferDetailsCommand>
{
    public UpdateTransferDetailsCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(r => r.TransferDetailDtos)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
    }
} 