namespace PetFamily.Species.Contracts.Requests;

public record BreedToSpeciesExistenceRequest(Guid SpeciesId, Guid BreedId);