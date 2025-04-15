using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class TestId : ValueObject, IComparable<TestId>
{
    public Guid Value { get; }

    private TestId(Guid value) => Value = value;

    public static TestId NewTestId() => new(Guid.NewGuid());
    public static TestId EmptyTestId => new(Guid.Empty);
    public static TestId Create(Guid id) => new(id);
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator TestId(Guid testId) => new (testId);
    public static implicit operator Guid(TestId testId) => testId.Value;

    public int CompareTo(TestId? other)
    {
        if (other == null)
            throw new Exception("TestId cannot be null");
        return Value.CompareTo(other.Value);
    }
}