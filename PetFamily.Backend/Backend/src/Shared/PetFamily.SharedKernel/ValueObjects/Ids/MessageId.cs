using CSharpFunctionalExtensions;

namespace PetFamily.SharedKernel.ValueObjects.Ids;

public class MessageId : ValueObject, IComparable<MessageId>
{
    public Guid Value { get; }

    private MessageId(Guid value) => Value = value;

    public static MessageId NewMessageId() => new(Guid.NewGuid());

    public static MessageId EmptyMessageId => new(Guid.Empty);

    public static MessageId Create(Guid id) => new(id);

    public static implicit operator MessageId(Guid messageId) => new(messageId);

    public static implicit operator Guid(MessageId messageId) => messageId.Value;

    public int CompareTo(MessageId? other)
    {
        if (other == null)
            throw new Exception("MessageId cannot be null");
        return Value.CompareTo(other.Value);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}