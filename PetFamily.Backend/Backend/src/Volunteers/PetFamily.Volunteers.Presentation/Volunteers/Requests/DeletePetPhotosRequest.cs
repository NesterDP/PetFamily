using PetFamily.Volunteers.Application.Commands.DeletePetPhotos;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record DeletePetPhotosRequest(IEnumerable<Guid> PhotosIds)
{
    public DeletePetPhotosCommand ToCommand(Guid volunteerId, Guid petId) => new(
        volunteerId,
        petId,
        PhotosIds.ToList());
}