using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Discussions.Domain.ValueObjects;

public record MessageText
{
    public string Value { get; }

    public MessageText(string value)
    {
        Value = value;
    }

    public static Result<MessageText, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > DomainConstants.MAX_MEDIUM_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("MessageText");

        return new MessageText(value);
    }
}