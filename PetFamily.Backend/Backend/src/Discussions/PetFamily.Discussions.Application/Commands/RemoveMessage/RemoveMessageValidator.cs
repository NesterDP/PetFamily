using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Application.Commands.RemoveMessage;

public class RemoveMessageCommandValidator : AbstractValidator<RemoveMessageCommand>
{
    public RemoveMessageCommandValidator()
    {
        RuleFor(c => c.RelationId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RelationId"));
        
        RuleFor(c => c.MessageId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("MessageId"));
        
        RuleFor(c => c.UserId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("UserId"));
    }
}