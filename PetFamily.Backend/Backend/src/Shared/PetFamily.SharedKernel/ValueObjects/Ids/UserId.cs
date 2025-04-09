using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class UserId : ValueObject, IComparable<UserId>
{
    public Guid Value { get; }

    private UserId(Guid value) => Value = value;

    public static UserId NewUserId() => new(Guid.NewGuid());
    public static UserId EmptyUserId => new(Guid.Empty);
    public static UserId Create(Guid id) => new(id);
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator UserId(Guid userId) => new (userId);
    public static implicit operator Guid(UserId userId) => userId.Value;

    public int CompareTo(UserId? other)
    {
        if (other == null)
            throw new Exception("UserId cannot be null");
        return Value.CompareTo(other.Value);
    }
}