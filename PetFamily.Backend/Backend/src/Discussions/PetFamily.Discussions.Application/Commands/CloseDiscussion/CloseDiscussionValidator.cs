using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.Discussions.Application.Commands.CloseDiscussion;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Application.Commands.CloseDiscussion;


public class CloseDiscussionCommandValidator : AbstractValidator<CloseDiscussionCommand>
{
    public CloseDiscussionCommandValidator()
    {
        RuleFor(c => c.RelationId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RelationId"));
    }
}