using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Address
{
    public string Value { get; }

    private Address(string value) => Value = value;

    public static Result<Address> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return Result.Failure<Address>("Address cannot be null or empty.");

        var validAddress = new Address(address);
        
        return Result.Success(validAddress);
    }
}