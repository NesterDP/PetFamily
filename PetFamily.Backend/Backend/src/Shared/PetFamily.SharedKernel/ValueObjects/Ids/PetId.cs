using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class PetId : ValueObject, IComparable<PetId>
{
    public Guid Value { get; }

    private PetId(Guid value) => Value = value;

    public static PetId NewPetId() => new(Guid.NewGuid());

    public static PetId EmptyPetId => new(Guid.Empty);

    public static PetId Create(Guid id) => new(id);

    public static implicit operator PetId(Guid petId) => new (petId);

    public static implicit operator Guid(PetId petId) => petId.Value;

    public int CompareTo(PetId? other)
    {
        if (other == null)
            throw new Exception("PetId cannot be null");
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}