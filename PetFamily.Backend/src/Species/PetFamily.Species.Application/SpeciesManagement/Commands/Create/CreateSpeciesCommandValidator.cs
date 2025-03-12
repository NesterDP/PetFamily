using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.Core.SharedVO;

namespace PetFamily.Species.Application.SpeciesManagement.Commands.Create;

public class CreateSpeciesCommandValidator : AbstractValidator<CreateSpeciesCommand>
{
    public CreateSpeciesCommandValidator()
    {
        RuleFor(s => s.Name).MustBeValueObject(Name.Create);
    }
}