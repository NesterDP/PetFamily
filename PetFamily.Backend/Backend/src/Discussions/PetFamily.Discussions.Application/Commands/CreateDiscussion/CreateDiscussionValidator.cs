using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Application.Commands.CreateDiscussion;

public class CreateDiscussionCommandValidator : AbstractValidator<CreateDiscussionCommand>
{
    public CreateDiscussionCommandValidator()
    {
        RuleFor(c => c.RelationId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RelationId"));

        RuleForEach(c => c.UserIds)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("UserId"));
    }
}