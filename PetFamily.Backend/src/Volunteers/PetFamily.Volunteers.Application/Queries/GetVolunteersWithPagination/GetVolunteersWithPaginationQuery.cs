using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Queries.GetVolunteersWithPagination;

public record GetVolunteersWithPaginationQuery(int Page, int PageSize) : IQuery;