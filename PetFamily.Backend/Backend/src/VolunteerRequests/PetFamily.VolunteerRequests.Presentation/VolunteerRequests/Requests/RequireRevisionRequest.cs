using PetFamily.VolunteerRequests.Application.Commands.RequireRevision;

namespace PetFamily.VolunteerRequests.Presentation.VolunteerRequests.Requests;

public record RequireRevisionRequest(string RevisionComment)
{
    public RequireRevisionCommand ToCommand(Guid requestId, Guid adminId) => new(requestId, adminId, RevisionComment);
}