using PetFamily.VolunteerRequests.Application.Queries.GetRequestsByUserId;

namespace PetFamily.VolunteerRequests.Presentation.VolunteerRequests.Requests;

public record GetRequestsByUserIdRequest(int Page, int PageSize, string? Status = null)
{
    public GetRequestsByUserIdQuery ToQuery(Guid userId) => new(userId, Page, PageSize, Status);
}