namespace PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

public record VolunteerId
{
    public Guid Value { get; }

    private VolunteerId(Guid value) => Value = value;
    
    public static VolunteerId NewVolunteerId() => new(Guid.NewGuid());
    public static VolunteerId EmptyVolunteerId => new(Guid.Empty);
    public static VolunteerId Create(Guid id) => new(id);
}