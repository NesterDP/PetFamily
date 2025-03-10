using PetFamily.Application.Species.Queries.GetSpeciesWithPagination;

namespace PetFamily.API.Controllers.Species.Requests;

public record GetSpeciesWithPaginationRequest(int Page, int PageSize)
{
    public GetSpeciesWithPaginationQuery ToQuery() => new (Page, PageSize);
}