using FluentValidation;
using PetFamily.API.Extensions;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateTransferDetails;

public class UpdateTransferDetailsValidator : AbstractValidator<UpdateTransferDetailsRequest>
{
    public UpdateTransferDetailsValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(r => r.Dto.TransferDetails)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
    }
} 