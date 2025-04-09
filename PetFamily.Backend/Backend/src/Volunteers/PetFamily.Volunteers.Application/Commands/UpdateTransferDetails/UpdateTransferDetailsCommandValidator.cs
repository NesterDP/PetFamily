using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Volunteers.Application.Commands.UpdateTransferDetails;

public class UpdateTransferDetailsCommandValidator : AbstractValidator<UpdateTransferDetailsCommand>
{
    public UpdateTransferDetailsCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(r => r.TransferDetailDtos)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
    }
} 