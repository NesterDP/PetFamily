using PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdatePetMainPhotoRequest(string MainPhotoPath)
{
    public UpdatePetMainPhotoCommand ToCommand(Guid volunteerId, Guid petId) =>
    new (volunteerId, petId, MainPhotoPath);
}