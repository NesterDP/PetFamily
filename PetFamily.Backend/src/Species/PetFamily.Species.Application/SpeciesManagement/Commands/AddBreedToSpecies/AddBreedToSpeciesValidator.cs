using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.Core.SharedVO;

namespace PetFamily.Species.Application.SpeciesManagement.Commands.AddBreedToSpecies;

public class AddBreedToSpeciesValidator : AbstractValidator<AddBreedToSpeciesCommand>
{
    public AddBreedToSpeciesValidator()
    {
        RuleFor(s => s.Name).MustBeValueObject(Name.Create);
    }
}