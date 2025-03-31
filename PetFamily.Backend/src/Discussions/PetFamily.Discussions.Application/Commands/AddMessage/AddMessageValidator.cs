using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Application.Commands.AddMessage;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator()
    {
        RuleFor(c => c.RelationId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("RelationId"));
        
        RuleFor(c => c.UserId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("UserId"));

        RuleFor(c => c.MessageText).MustBeValueObject(MessageText.Create);
    }
}