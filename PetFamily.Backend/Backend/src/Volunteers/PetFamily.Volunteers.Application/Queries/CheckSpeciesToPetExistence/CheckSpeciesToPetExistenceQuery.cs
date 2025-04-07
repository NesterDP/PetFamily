using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Queries.CheckSpeciesToPetExistence;

public record CheckSpeciesToPetExistenceQuery(Guid SpeciesId) : IQuery;