using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.VolunteerRequests.Application.Commands.ApproveRequest;

public class ApproveRequestCommandValidator : AbstractValidator<ApproveRequestCommand>
{
    public ApproveRequestCommandValidator()
    {
        RuleFor(c => c.RequestId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RequestId"));

        RuleFor(c => c.AdminId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("AdminId"));
    }
}