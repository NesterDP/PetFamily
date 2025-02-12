using CSharpFunctionalExtensions;
namespace PetFamily.Domain.PetContext.ValueObjects.PetVO;

public record PetClassification
{
    public Guid BreedId { get; }
    public Guid SpeciesId { get; }

    private PetClassification(Guid breedId, Guid speciesId)
    {
        BreedId = breedId;
        SpeciesId = speciesId;
    }

    public static Result<PetClassification> Create(Guid breedId, Guid speciesId)
    {
        if (breedId == Guid.Empty)
            return Result.Failure<PetClassification>("breedId cannot be empty.");
        if (speciesId == Guid.Empty)
            return Result.Failure<PetClassification>("speciesId cannot be empty.");

        var validPetClassification = new PetClassification(breedId, speciesId);
        return Result.Success(validPetClassification);
    }
}