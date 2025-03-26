using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.VolunteerRequests.Domain.ValueObjects;

public record VolunteerInfo
{
    public string Value { get; }

    public VolunteerInfo(string value)
    {
        Value = value;
    }

    public static Result<VolunteerInfo, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > DomainConstants.MAX_HIGH_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("VolunteerInfo");
        
        return new VolunteerInfo(value);
    }
}