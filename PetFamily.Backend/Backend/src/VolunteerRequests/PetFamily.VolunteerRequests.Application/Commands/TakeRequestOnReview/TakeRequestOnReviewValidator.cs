using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeRequestOnReview;

public class TakeRequestOnReviewCommandValidator : AbstractValidator<TakeRequestOnReviewCommand>
{
    public TakeRequestOnReviewCommandValidator()
    {
        RuleFor(c => c.AdminId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("AdminId"));

        RuleFor(c => c.RequestId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RequestId"));
    }
}