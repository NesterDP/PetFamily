using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

public record Experience
{
    public int Value { get; }

    private Experience(int value) => Value = value;

    public static Result<Experience, Error> Create(int experience)
    {
        if (experience < 0)
            return Errors.General.ValueIsInvalid("experience");

        var validExperience = new Experience(experience);
        
        return validExperience;
    }
}