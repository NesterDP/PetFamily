using PetFamily.Core.Abstractions;

namespace PetFamily.Species.Application.Queries.GetSpeciesWithPagination;

public record GetSpeciesWithPaginationQuery(int Page, int PageSize) : IQuery;