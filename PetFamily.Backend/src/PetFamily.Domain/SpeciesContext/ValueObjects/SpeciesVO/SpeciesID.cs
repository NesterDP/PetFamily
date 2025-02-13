namespace PetFamily.Domain.SpeciesContext.ValueObjects.SpeciesVO;

public record SpeciesId
{
    public Guid Value { get; }

    private SpeciesId(Guid value) => Value = value;
    
    public static SpeciesId NewSpeciesId() => new(Guid.NewGuid());
    public static SpeciesId EmptySpeciesId => new(Guid.Empty);
    public static SpeciesId Create(Guid id) => new(id);
}