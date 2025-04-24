using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class DiscussionId : ValueObject, IComparable<DiscussionId>
{
    public Guid Value { get; }

    private DiscussionId(Guid value) => Value = value;

    public static DiscussionId NewDiscussionId() => new(Guid.NewGuid());

    public static DiscussionId EmptyDiscussionId => new(Guid.Empty);

    public static DiscussionId Create(Guid id) => new(id);

    public static implicit operator DiscussionId(Guid discussionId) => new(discussionId);

    public static implicit operator Guid(DiscussionId discussionId) => discussionId.Value;

    public int CompareTo(DiscussionId? other)
    {
        if (other == null)
            throw new Exception("DiscussionId cannot be null");
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}