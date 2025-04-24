using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class SpeciesId : ValueObject, IComparable<SpeciesId>
{
    public Guid Value { get; }

    private SpeciesId(Guid value) => Value = value;

    public static SpeciesId NewSpeciesId() => new(Guid.NewGuid());

    public static SpeciesId EmptySpeciesId => new(Guid.Empty);

    public static SpeciesId Create(Guid id) => new(id);

    public static implicit operator SpeciesId(Guid speciesId) => new (speciesId);

    public static implicit operator Guid(SpeciesId speciesId) => speciesId.Value;

    public int CompareTo(SpeciesId? other)
    {
        if (other == null)
            throw new Exception("SpeciesId cannot be null");
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}