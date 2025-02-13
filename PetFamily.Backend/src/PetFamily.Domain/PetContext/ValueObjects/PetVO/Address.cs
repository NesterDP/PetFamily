using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Address
{
    public string Value { get; }

    private Address(string value) => Value = value;

    public static Result<Address> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || address.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return Result.Failure<Address>("Address is invalid");

        var validAddress = new Address(address);
        
        return Result.Success(validAddress);
    }
}