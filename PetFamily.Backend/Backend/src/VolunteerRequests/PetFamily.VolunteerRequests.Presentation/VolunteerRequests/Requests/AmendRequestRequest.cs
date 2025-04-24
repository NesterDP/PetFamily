using PetFamily.VolunteerRequests.Application.Commands.AmendRequest;

namespace PetFamily.VolunteerRequests.Presentation.VolunteerRequests.Requests;

public record AmendRequestRequest(string UpdatedInfo)
{
    public AmendRequestCommand ToCommand(Guid requestId, Guid userId) => new(requestId, userId, UpdatedInfo);
}