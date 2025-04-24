namespace PetFamily.Core.Dto.Pet;

public record PetClassificationDto
{
    public PetClassificationDto(Guid speciesId, Guid breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }

    public Guid SpeciesId { get; set; }
    public Guid BreedId { get; set; }
}