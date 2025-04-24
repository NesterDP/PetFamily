using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class RelationId : ValueObject, IComparable<RelationId>
{
    public Guid Value { get; }

    private RelationId(Guid value) => Value = value;

    public static RelationId NewRelationId() => new(Guid.NewGuid());

    public static RelationId EmptyRelationId => new(Guid.Empty);

    public static RelationId Create(Guid id) => new(id);

    public static implicit operator RelationId(Guid relationId) => new (relationId);

    public static implicit operator Guid(RelationId relationId) => relationId.Value;

    public int CompareTo(RelationId? other)
    {
        if (other == null)
            throw new Exception("RelationId cannot be null");
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}