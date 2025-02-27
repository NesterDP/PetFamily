using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;

namespace PetFamily.Application.Volunteers.ChangePetPosition;

public class ChangePetPositionCommandValidator : AbstractValidator<ChangePetPositionCommand>
{
    public ChangePetPositionCommandValidator()
    {
        RuleFor(c => c.Position).MustBeValueObject(Position.Create);
    }
}