using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Species.Application.Commands.AddBreedToSpecies;

public class AddBreedToSpeciesValidator : AbstractValidator<AddBreedToSpeciesCommand>
{
    public AddBreedToSpeciesValidator()
    {
        RuleFor(s => s.Name).MustBeValueObject(Name.Create);
    }
}