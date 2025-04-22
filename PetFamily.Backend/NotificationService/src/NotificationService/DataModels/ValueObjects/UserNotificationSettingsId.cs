using CSharpFunctionalExtensions;

namespace NotificationService.DataModels.ValueObjects;

public class UserNotificationSettingsId : ValueObject, IComparable<UserNotificationSettingsId>
{
    public Guid Value { get; }

    private UserNotificationSettingsId(Guid value) => Value = value;

    public static UserNotificationSettingsId NewId() => new(Guid.NewGuid());
    public static UserNotificationSettingsId EmptyId => new(Guid.Empty);
    public static UserNotificationSettingsId Create(Guid id) => new(id);
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator UserNotificationSettingsId(Guid id) => new (id);
    
    public static implicit operator Guid(UserNotificationSettingsId id) => id.Value;

    public int CompareTo(UserNotificationSettingsId? other)
    {
        if (other == null)
            throw new Exception("UserNotificationSettingsId cannot be null");
        return Value.CompareTo(other.Value);
    }
}