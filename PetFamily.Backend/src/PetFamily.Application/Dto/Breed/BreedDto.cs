namespace PetFamily.Application.Dto.Breed;

public class BreedDto
{
    public Guid Id { get; init; }
    public Guid SpeciesId { get; init; }
    public string Name { get; init; }
}