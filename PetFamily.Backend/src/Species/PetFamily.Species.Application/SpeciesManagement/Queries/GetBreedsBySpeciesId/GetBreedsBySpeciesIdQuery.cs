using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.SpeciesManagement.Queries.GetBreedsBySpeciesId;

public record GetBreedsBySpeciesIdQuery(Guid Id) : IQuery;