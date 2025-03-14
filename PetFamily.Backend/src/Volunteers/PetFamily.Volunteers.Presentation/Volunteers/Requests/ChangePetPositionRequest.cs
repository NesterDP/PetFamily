using PetFamily.Volunteers.Application.Commands.ChangePetPosition;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record ChangePetPositionRequest(int Position)
{
    public ChangePetPositionCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new ChangePetPositionCommand(volunteerId, petId, Position);
    }
}