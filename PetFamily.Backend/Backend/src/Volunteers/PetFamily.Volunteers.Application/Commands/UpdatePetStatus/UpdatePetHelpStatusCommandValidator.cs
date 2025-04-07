using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;

namespace PetFamily.Volunteers.Application.Commands.UpdatePetStatus;

public class UpdatePetHelpStatusCommandValidator : AbstractValidator<UpdatePetHelpStatusCommand>
{
    public UpdatePetHelpStatusCommandValidator()
    {
        RuleFor(c => c.HelpStatus).MustBeValueObject(HelpStatus.Create);
    }
}