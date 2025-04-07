using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.Queries.GetBreedsBySpeciesId;

public record GetBreedsBySpeciesIdQuery(Guid Id) : IQuery;