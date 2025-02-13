using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record HealthInfo
{
    public string Value { get; }

    private HealthInfo(string value) => Value = value;

    public static Result<HealthInfo> Create(string healthInfo)
    {
        if (string.IsNullOrWhiteSpace(healthInfo) || healthInfo.Length > Constants.MAX_MEDIUM_TEXT_LENGTH)
            return Result.Failure<HealthInfo>("HealthInfo is invalid");

        var validHealthInfo = new HealthInfo(healthInfo);
        
        return Result.Success(validHealthInfo);
    }
}