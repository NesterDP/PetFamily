using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public class VolunteerId : ValueObject, IComparable<VolunteerId>
{
    public Guid Value { get; }

    private VolunteerId(Guid value) => Value = value;

    public static VolunteerId NewVolunteerId() => new(Guid.NewGuid());
    public static VolunteerId EmptyVolunteerId => new(Guid.Empty);
    public static VolunteerId Create(Guid id) => new(id);
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator VolunteerId(Guid volunteerId) => new (volunteerId);
    public static implicit operator Guid(VolunteerId volunteerId) => volunteerId.Value;

    public int CompareTo(VolunteerId? other)
    {
        if (other == null)
            throw new Exception("VolunteerId cannot be null");
        return Value.CompareTo(other.Value);
    }
}