using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.SharedKernel.ValueObjects;

public class FullName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string? Surname { get; }

    private FullName(string firstName, string lastName, string? surname = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Surname = surname;
    }

    public static Result<FullName, Error> Create(string firstName, string lastName, string? surname = null)
    {
        
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > DomainConstants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("firstName");
        
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > DomainConstants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("lastName");
        
        if (surname?.Length > DomainConstants.MAX_NAME_LENGTH)
            return Errors.General.ValueIsInvalid("surname");
        
        var validFullname = new FullName(firstName, lastName, surname);
        return validFullname;
        
    }
}