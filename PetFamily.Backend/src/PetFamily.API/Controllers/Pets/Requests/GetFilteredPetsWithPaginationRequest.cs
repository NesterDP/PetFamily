using PetFamily.Application.Pets.Queries.GetFilteredPetsWithPagination;

namespace PetFamily.API.Controllers.Pets.Requests;

public record GetFilteredPetsWithPaginationRequest(int Page, int PageSize)
{
    public GetFilteredPetsWithPaginationQuery ToQuery() => new (Page, PageSize);
}