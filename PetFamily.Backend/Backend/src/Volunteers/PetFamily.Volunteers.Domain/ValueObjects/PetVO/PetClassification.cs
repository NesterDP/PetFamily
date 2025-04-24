using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Domain.ValueObjects.PetVO;

public record PetClassification
{
    public Guid SpeciesId { get; }

    public Guid BreedId { get; }

    private PetClassification(Guid speciesId, Guid breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }

    public static Result<PetClassification, Error> Create(Guid speciesId, Guid breedId)
    {
        if (speciesId == Guid.Empty)
            return Errors.General.ValueIsInvalid("speciesId");
        if (breedId == Guid.Empty)
            return Errors.General.ValueIsInvalid("breedId");

        var validPetClassification = new PetClassification(speciesId, breedId);
        return validPetClassification;
    }
}