using PetFamily.Volunteers.Application.VolunteersManagement.Commands.DeletePetPhotos;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record DeletePetPhotosRequest(IEnumerable<string> PhotosNames)
{
    public DeletePetPhotosCommand ToCommand(Guid volunteerId, Guid petId) => new(
        volunteerId,
        petId,
        PhotosNames.ToList());
}