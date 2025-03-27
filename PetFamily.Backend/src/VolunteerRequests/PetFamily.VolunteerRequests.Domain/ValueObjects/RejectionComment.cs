using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.VolunteerRequests.Domain.ValueObjects;

public record RejectionComment
{
    public string Value { get; }

    public RejectionComment(string value)
    {
        Value = value;
    }

    public static Result<RejectionComment, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > DomainConstants.MAX_MEDIUM_TEXT_LENGTH)
            return Errors.General.ValueIsInvalid("RejectionComment");
        
        return new RejectionComment(value);
    }
}