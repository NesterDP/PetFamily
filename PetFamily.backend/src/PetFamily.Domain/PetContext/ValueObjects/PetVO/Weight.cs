using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Weight
{
    public float Value { get; }

    private Weight(float value) => Value = value;

    public static Result<Weight> Create(float weight)
    {
        if (weight <= 0)
            return Result.Failure<Weight>("Weight must be greater than 0");

        var validWeight = new Weight(weight);
        
        return Result.Success(validWeight);
    }
}