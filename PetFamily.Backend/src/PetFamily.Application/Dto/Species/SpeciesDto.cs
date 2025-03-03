using PetFamily.Application.Dto.Breed;

namespace PetFamily.Application.Dto.Species;

public class SpeciesDto
{
    public Guid Id { get; init; }
    
    public string Name { get; init; }
    
    public IReadOnlyList<BreedDto> Breeds { get; init; }
}