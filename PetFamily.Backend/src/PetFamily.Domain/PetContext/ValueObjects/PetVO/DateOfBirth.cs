using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record DateOfBirth
{
    public DateTime Value { get; }
    

    private DateOfBirth(DateTime value) => Value = value;

    public static Result<DateOfBirth, Error> Create(DateTime dateOfBirth)
    {
        // родился в будущем или родился слишком давно
        if (dateOfBirth > DateTime.Now || dateOfBirth < DateTime.Now.AddYears(-30))
            return Errors.General.ValueIsInvalid("dateOfBirth");

        var validDateOfBirth = new DateOfBirth(dateOfBirth);
        
        return validDateOfBirth;
    }
}