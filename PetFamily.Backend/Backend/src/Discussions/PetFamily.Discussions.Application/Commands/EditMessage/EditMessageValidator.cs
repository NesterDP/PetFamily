using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Application.Commands.EditMessage;

public class EditMessageCommandValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageCommandValidator()
    {
        RuleFor(c => c.RelationId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RelationId"));

        RuleFor(c => c.MessageId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("MessageId"));

        RuleFor(c => c.UserId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("UserId"));

        RuleFor(c => c.NewMessageText).MustBeValueObject(MessageText.Create);
    }
}