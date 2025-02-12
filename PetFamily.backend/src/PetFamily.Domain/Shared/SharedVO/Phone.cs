using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record Phone
{
    public string Value { get; }

    private Phone(string value) => Value = value;

    public static Result<Phone> Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return Result.Failure<Phone>("Phone cannot be null or empty.");

        var validPhone = new Phone(phone);
        
        return Result.Success(validPhone);
    }
}