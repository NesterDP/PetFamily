using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

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

    public static Result<FullName> Create(string firstName, string lastName, string surname)
    {
        
        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > Constants.MAX_NAME_LENGTH)
            return Result.Failure<FullName>("Invalid first name");
        
        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > Constants.MAX_NAME_LENGTH)
            return Result.Failure<FullName>("Invalid last name");
        
        if (string.IsNullOrWhiteSpace(surname) || surname.Length > Constants.MAX_NAME_LENGTH)
            return Result.Failure<FullName>("Invalid surname");
        
        var validFio = new FullName(firstName, lastName, surname);
        return Result.Success(validFio);
        
    }
}