using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Queries.CheckBreedToPetExistence;


public record CheckBreedToPetExistenceQuery(Guid BreedId) : IQuery;