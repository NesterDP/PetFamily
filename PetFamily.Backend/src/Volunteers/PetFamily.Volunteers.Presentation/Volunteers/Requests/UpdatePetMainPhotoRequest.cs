using PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdatePetMainPhoto;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UpdatePetMainPhotoRequest(string MainPhotoPath)
{
    public UpdatePetMainPhotoCommand ToCommand(Guid volunteerId, Guid petId) =>
    new (volunteerId, petId, MainPhotoPath);
}