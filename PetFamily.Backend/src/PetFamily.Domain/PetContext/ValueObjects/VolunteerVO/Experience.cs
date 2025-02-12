using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public record Experience
{
    public int Value { get; }

    private Experience(int value) => Value = value;

    public static Result<Experience> Create(int experience)
    {
        if (experience < 0)
            return Result.Failure<Experience>("Experience cannot be negative");

        var validExperience = new Experience(experience);
        
        return Result.Success(validExperience);
    }
}