using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record HealthInfo
{
    public string Value { get; }

    private HealthInfo(string value) => Value = value;

    public static Result<HealthInfo> Create(string healthInfo)
    {
        if (string.IsNullOrWhiteSpace(healthInfo))
            return Result.Failure<HealthInfo>("HealthInfo cannot be null or empty.");

        var validHealthInfo = new HealthInfo(healthInfo);
        
        return Result.Success(validHealthInfo);
    }
}