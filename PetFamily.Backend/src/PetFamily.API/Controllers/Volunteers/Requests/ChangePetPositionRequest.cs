using PetFamily.Application.Volunteers.ChangePetPosition;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record ChangePetPositionRequest(int Position)
{
    public ChangePetPositionCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new ChangePetPositionCommand(volunteerId, petId, Position);
    }
}