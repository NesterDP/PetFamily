using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class VolunteerRequestId : ValueObject, IComparable<VolunteerRequestId>
{
    public Guid Value { get; }

    private VolunteerRequestId(Guid value) => Value = value;

    public static VolunteerRequestId NewVolunteerRequestId() => new(Guid.NewGuid());
    public static VolunteerRequestId EmptyVolunteerRequestId => new(Guid.Empty);
    public static VolunteerRequestId Create(Guid id) => new(id);
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator VolunteerRequestId(Guid VolunteerRequestId) => new (VolunteerRequestId);
    public static implicit operator Guid(VolunteerRequestId VolunteerRequestId) => VolunteerRequestId.Value;

    public int CompareTo(VolunteerRequestId? other)
    {
        if (other == null)
            throw new Exception("VolunteerRequestId cannot be null");
        return Value.CompareTo(other.Value);
    }
}