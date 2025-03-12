using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public class Position : ValueObject
{
    public static Position First = new(1);
    public int Value { get; }

    private Position(int value) => Value = value;

    public Result<Position, Error> Forward()
        => Create(Value + 1);
    
    public Result<Position, Error> Backward()
        => Create(Value - 1);

    public static Result<Position, Error> Create(int position)
    {
        if (position < 1)
            return Errors.General.ValueIsInvalid("position");

        var validSerialNumber = new Position(position);
        
        return validSerialNumber;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator int(Position position) => position.Value;
    
}