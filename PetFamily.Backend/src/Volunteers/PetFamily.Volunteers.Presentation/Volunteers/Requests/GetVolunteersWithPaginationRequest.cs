using PetFamily.Volunteers.Application.VolunteersManagement.Queries.GetVolunteersWithPagination;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record GetVolunteersWithPaginationRequest(int Page, int PageSize)
{
    public GetVolunteersWithPaginationQuery ToQuery() => new (Page, PageSize);
}