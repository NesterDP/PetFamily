using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class Fio
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Surname { get; }

    private Fio(string firstName, string lastName, string surname)
    {
        FirstName = firstName;
        LastName = lastName;
        Surname = surname;
    }

    public static Result<Fio> Create(string firstName, string lastName, string surname)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<Fio>("FirstName cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<Fio>("LastName cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(surname))
            return Result.Failure<Fio>("Surname cannot be null or empty.");
        
        var validFio = new Fio(firstName, lastName, surname);
        return Result.Success(validFio);
        
    }
}