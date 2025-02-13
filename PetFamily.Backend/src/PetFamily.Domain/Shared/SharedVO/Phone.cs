using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
namespace PetFamily.Domain.Shared.SharedVO;

public record Phone
{
    public string Value { get; }

    private Phone(string value) => Value = value;

    public static Result<Phone> Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length > Constants.MAX_PHONE_LENGTH)
            return Result.Failure<Phone>("Phone is either empty or too long");
        
        const string pattern = @"^\d-\d{3}-\d{2}-\d{2}-\d{2}$";
        if (!Regex.IsMatch(phone, pattern))
            return Result.Failure<Phone>("Phone is in incorrect format");

        var validPhone = new Phone(phone);
        
        return Result.Success(validPhone);
    }
}