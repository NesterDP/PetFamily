using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.CustomErrors;
namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class FullName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Surname { get; }

    private FullName(string firstName, string lastName, string surname)
    {
        FirstName = firstName;
        LastName = lastName;
        Surname = surname;
    }

    public static Result<FullName, Error> Create(string firstName, string lastName, string surname)
    {
        
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > Constants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("firstName");
        
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > Constants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("lastName");
        
        if (string.IsNullOrWhiteSpace(surname) || surname.Length > Constants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("surname");
        
        var validFullname = new FullName(firstName, lastName, surname);
        return validFullname;
        
    }
}