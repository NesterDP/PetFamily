using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared.CustomErrors;

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

    public static Result<PetClassification, Error> Create(Guid breedId, Guid speciesId)
    {
        if (breedId == Guid.Empty)
            return Errors.General.ValueIsInvalid("breedId");
        if (speciesId == Guid.Empty)
            return Errors.General.ValueIsInvalid("speciesId");

        var validPetClassification = new PetClassification(breedId, speciesId);
        return validPetClassification;
    }
}