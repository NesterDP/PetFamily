using CSharpFunctionalExtensions;
using PetFamily.SharedKernel;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public record HealthInfo
{
    public string Value { get; }

    private HealthInfo(string value) => Value = value;

    public static Result<HealthInfo, Error> Create(string healthInfo)
    {
        if (string.IsNullOrWhiteSpace(healthInfo) || healthInfo.Length > SharedConstants.MAX_MEDIUM_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("healthInfo");

        var validHealthInfo = new HealthInfo(healthInfo);
        
        return validHealthInfo;
    }
}