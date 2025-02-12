using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>("Email cannot be null or empty.");

        var validEmail = new Email(email);
        
        return Result.Success(validEmail);
    }
}