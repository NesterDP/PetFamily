using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class BreedId : ValueObject, IComparable<BreedId>
{
    public Guid Value { get; }

    private BreedId(Guid value) => Value = value;

    public static BreedId NewBreedId() => new(Guid.NewGuid());

    public static BreedId EmptyBreedId => new(Guid.Empty);

    public static BreedId Create(Guid id) => new(id);

    public static implicit operator BreedId(Guid breedId) => new (breedId);

    public static implicit operator Guid(BreedId breedId) => breedId.Value;

    public int CompareTo(BreedId? other)
    {
        if (other == null)
            throw new Exception("BreedId cannot be null");
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}