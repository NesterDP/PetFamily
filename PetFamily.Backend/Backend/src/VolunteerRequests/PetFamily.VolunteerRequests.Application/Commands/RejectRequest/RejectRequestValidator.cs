using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.VolunteerRequests.Application.Commands.RejectRequest;

public class RejectRequestCommandValidator : AbstractValidator<RejectRequestCommand>
{
    public RejectRequestCommandValidator()
    {
        RuleFor(c => c.RequestId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RequestId"));

        RuleFor(c => c.AdminId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("AdminId"));
    }
}