namespace PetFamily.Application.Dto.SpeciesDto;

public class SpeciesDto
{
    public Guid Id { get; init; }
    
    public string Name { get; init; }
    
    public IReadOnlyList<BreedDto.BreedDto> Breeds { get; init; }
}