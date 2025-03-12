using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Volunteers.Queries.GetVolunteersWithPagination;

public record GetVolunteersWithPaginationQuery(int Page, int PageSize) : IQuery;