using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record IsVaccinated
{
    public bool Value { get; }

    private IsVaccinated(bool value) => Value = value;

    public static Result<IsVaccinated> Create(bool isVaccinated)
    {
        var validIsVaccinated = new IsVaccinated(isVaccinated);
        
        return Result.Success(validIsVaccinated);
    }
}