using PetFamily.Volunteers.Application.Commands.UpdatePetStatus;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UpdatePetHelpStatusRequest(string HelpStatus)
{
    public UpdatePetHelpStatusCommand ToCommand(Guid volunteerId, Guid petId) =>
        new(volunteerId, petId, HelpStatus);
}