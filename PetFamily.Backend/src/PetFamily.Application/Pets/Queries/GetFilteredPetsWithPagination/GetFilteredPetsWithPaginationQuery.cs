using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;

public record GetFilteredPetsWithPaginationQuery(int Page, int PageSize) : IQuery;