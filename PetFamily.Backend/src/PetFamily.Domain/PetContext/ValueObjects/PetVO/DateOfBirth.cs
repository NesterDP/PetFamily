using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record DateOfBirth
{
    public DateTime Value { get; }
    

    private DateOfBirth(DateTime value) => Value = value;

    public static Result<DateOfBirth> Create(DateTime dateOfBirth)
    {
        // родился в будущем или родился слишком давно
        if (dateOfBirth > DateTime.Now || dateOfBirth < DateTime.Now.AddYears(-30))
            return Result.Failure<DateOfBirth>("Incorrect date of birth");

        var validDateOfBirth = new DateOfBirth(dateOfBirth);
        
        return Result.Success(validDateOfBirth);
    }
}