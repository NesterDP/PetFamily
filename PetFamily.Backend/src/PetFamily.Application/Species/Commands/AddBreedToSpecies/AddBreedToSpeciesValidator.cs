using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Species.Commands.AddBreedToSpecies;

public class AddBreedToSpeciesValidator : AbstractValidator<AddBreedToSpeciesCommand>
{
    public AddBreedToSpeciesValidator()
    {
        RuleFor(s => s.Name).MustBeValueObject(Name.Create);
    }
}