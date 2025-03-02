using PetFamily.Application.Volunteers.UseCases.DeletePetPhotos;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record DeletePetPhotosRequest(IEnumerable<string> PhotosNames)
{
    public DeletePetPhotosCommand ToCommand(Guid volunteerId, Guid petId) => new(
        volunteerId,
        petId,
        PhotosNames.ToList());
}