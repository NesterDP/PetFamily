using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.Queries.CheckBreedToSpeciesExistence;

public record CheckBreedToSpeciesExistenceQuery(Guid SpeciesId, Guid BreedId) : IQuery;