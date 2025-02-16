using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record Address
{
    public string City { get; }
    public string House { get; }
    public string? Apartment { get; }

    private Address(string city, string house, string? apartment)
    {
        City = city;
        House = house;
        Apartment = apartment;
    }

    public static Result<Address, Error> Create(string city, string house, string? apartment)
    {
        if (string.IsNullOrWhiteSpace(city) || city.Length > Constants.MAX_LOGISTIC_UNIT_LENGTH)
            return Errors.General.ValueIsInvalid("city");
        
        if (string.IsNullOrWhiteSpace(house) || house.Length > Constants.MAX_LOGISTIC_UNIT_LENGTH)
            return Errors.General.ValueIsInvalid("house");
        
        if (string.IsNullOrWhiteSpace(apartment) || apartment.Length > Constants.MAX_LOGISTIC_UNIT_LENGTH)
            return Errors.General.ValueIsInvalid("apartment");

        var validAddress = new Address(city, house, apartment);
        
        return validAddress;
    }
}