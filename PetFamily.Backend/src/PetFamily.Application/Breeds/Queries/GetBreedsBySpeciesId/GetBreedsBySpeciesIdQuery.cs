using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Breeds.Queries.GetBreedsBySpeciesId;

public record GetBreedsBySpeciesIdQuery(Guid Id) : IQuery;