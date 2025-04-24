using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public record IsVaccinated
{
    public bool Value { get; }

    private IsVaccinated(bool value) => Value = value;

    public static Result<IsVaccinated, Error> Create(bool isVaccinated)
    {
        var validIsVaccinated = new IsVaccinated(isVaccinated);

        return validIsVaccinated;
    }
}