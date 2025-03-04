using PetFamily.Application.Volunteers.Commands.UpdatePetStatus;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdatePetHelpStatusRequest(string HelpStatus)
{
    public UpdatePetHelpStatusCommand ToCommand(Guid volunteerId, Guid petId) =>
        new (volunteerId, petId, HelpStatus);
}