using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Species.Queries.GetSpeciesWithPagination;

public record GetSpeciesWithPaginationQuery(int Page, int PageSize) : IQuery;