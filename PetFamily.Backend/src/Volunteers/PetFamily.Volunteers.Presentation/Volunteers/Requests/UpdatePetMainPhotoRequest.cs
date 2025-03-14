using PetFamily.Volunteers.Application.Commands.UpdatePetMainPhoto;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UpdatePetMainPhotoRequest(string MainPhotoPath)
{
    public UpdatePetMainPhotoCommand ToCommand(Guid volunteerId, Guid petId) =>
    new (volunteerId, petId, MainPhotoPath);
}