using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.RequireRevision;

public class RequireRevisionCommandValidator : AbstractValidator<RequireRevisionCommand>
{
    public RequireRevisionCommandValidator()
    {
        RuleFor(c => c.RequestId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RequestId"));

        RuleFor(c => c.AdminId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("AdminId"));

        RuleFor(c => c.RevisionComment).MustBeValueObject(RevisionComment.Create);
    }
}