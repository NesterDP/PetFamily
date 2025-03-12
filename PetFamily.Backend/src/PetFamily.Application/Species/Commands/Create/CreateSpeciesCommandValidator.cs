using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Species.Commands.Create;

public class CreateSpeciesCommandValidator : AbstractValidator<CreateSpeciesCommand>
{
    public CreateSpeciesCommandValidator()
    {
        RuleFor(s => s.Name).MustBeValueObject(Name.Create);
    }
}