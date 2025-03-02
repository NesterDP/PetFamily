using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UseCases.UpdateTransferDetails;

public class UpdateTransferDetailsCommandValidator : AbstractValidator<UpdateTransferDetailsCommand>
{
    public UpdateTransferDetailsCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(r => r.TransferDetailDtos)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
    }
} 