namespace PetFamily.Core.Dto.Species;

public class SpeciesDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    // public IReadOnlyList<BreedDto> Breeds { get; init; }
}