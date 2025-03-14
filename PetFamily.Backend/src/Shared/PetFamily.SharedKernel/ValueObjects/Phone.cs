using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.SharedKernel.ValueObjects;

public record Phone
{
    public string Value { get; }

    private Phone(string value) => Value = value;

    public static Result<Phone, Error> Create(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length > SharedConstants.MAX_PHONE_LENGTH)
            return Errors.General.ValueIsInvalid("phone");
        
        const string pattern = @"^\d-\d-\d{3}-\d{2}-\d{2}-\d{2}$";
        if (!Regex.IsMatch(phone, pattern))
            return Errors.General.ValueIsInvalid("phone");

        var validPhone = new Phone(phone);
        
        return validPhone;
    }
}