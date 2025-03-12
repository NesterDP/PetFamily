using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.Shared.SharedVO;

public record Description
{
    public string Value { get; }

    private Description(string value) => Value = value;

    public static Result<Description, Error> Create(string description)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Length > DomainConstants.MAX_HIGH_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("description");

        var validDescription = new Description(description);
        
        return validDescription;
    }
}