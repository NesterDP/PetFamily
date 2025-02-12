using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Height
{
    public float Value { get; }

    private Height(float value) => Value = value;

    public static Result<Height> Create(float height)
    {
        if (height <= 0)
            return Result.Failure<Height>("Height must be greater than 0");

        var validHeight = new Height(height);
        
        return Result.Success(validHeight);
    }
}