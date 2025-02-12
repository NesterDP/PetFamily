using CSharpFunctionalExtensions;

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
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<FullName>("FirstName cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<FullName>("LastName cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(surname))
            return Result.Failure<FullName>("Surname cannot be null or empty.");
        
        var validFio = new FullName(firstName, lastName, surname);
        return Result.Success(validFio);
        
    }
}