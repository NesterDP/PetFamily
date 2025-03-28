using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.AmendRequest;

public class ReviseRequestCommandValidator : AbstractValidator<AmendRequestCommand>
{
    public ReviseRequestCommandValidator()
    {
        RuleFor(c => c.RequestId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RequestId"));
        
        RuleFor(c => c.UserId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("UserId"));

        RuleFor(c => c.UpdatedInfo).MustBeValueObject(VolunteerInfo.Create);
    }
}