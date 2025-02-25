namespace PetFamily.API.Controllers.Volunteers.Requests;

public record DeletePetPhotosRequest(IEnumerable<string> PhotosNames);