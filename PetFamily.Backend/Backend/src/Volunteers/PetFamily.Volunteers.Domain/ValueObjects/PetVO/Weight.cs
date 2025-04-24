using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public record Weight
{
    public float Value { get; }

    private Weight(float value) => Value = value;

    public static Result<Weight, Error> Create(float weight)
    {
        if (weight <= 0)
            return Errors.General.ValueIsInvalid("weight");

        var validWeight = new Weight(weight);

        return validWeight;
    }
}