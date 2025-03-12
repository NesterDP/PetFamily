using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public record Height
{
    public float Value { get; }

    private Height(float value) => Value = value;

    public static Result<Height, Error> Create(float height)
    {
        if (height <= 0)
            return Errors.General.ValueIsInvalid("height");

        var validHeight = new Height(height);
        
        return validHeight;
    }
}