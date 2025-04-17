namespace PetFamily.Volunteers.Contracts.Messaging;

public record BreedToSpeciesExistenceEvent(Guid SpeciesId, Guid BreedId);