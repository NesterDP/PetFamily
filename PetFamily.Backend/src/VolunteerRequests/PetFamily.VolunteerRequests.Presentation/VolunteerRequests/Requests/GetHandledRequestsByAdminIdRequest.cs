using PetFamily.VolunteerRequests.Application.Queries.GetHandledRequestsByAdminId;

namespace PetFamily.VolunteerRequests.Presentation.VolunteerRequests.Requests;

public record GetHandledRequestsByAdminIdRequest(int Page, int PageSize, string? Status = null)
{
    public GetHandledRequestsByAdminIdQuery ToQuery(Guid adminId) => new(adminId, Page, PageSize, Status);
}