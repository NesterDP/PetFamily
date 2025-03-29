using PetFamily.VolunteerRequests.Application.Queries.GetUnhandledRequests;

namespace PetFamily.VolunteerRequests.Presentation.VolunteerRequests.Requests;

public record GetUnhandledRequestsRequest(int Page, int PageSize)
{
    public GetUnhandledRequestsQuery ToQuery() => new (Page, PageSize);
}