using PetFamily.Species.Application.SpeciesManagement.Queries.GetSpeciesWithPagination;

namespace PetFamily.Species.Presentation.Species.Requests;

public record GetSpeciesWithPaginationRequest(int Page, int PageSize)
{
    public GetSpeciesWithPaginationQuery ToQuery() => new (Page, PageSize);
}