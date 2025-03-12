using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.SpeciesManagement.Queries.GetSpeciesWithPagination;

public record GetSpeciesWithPaginationQuery(int Page, int PageSize) : IQuery;