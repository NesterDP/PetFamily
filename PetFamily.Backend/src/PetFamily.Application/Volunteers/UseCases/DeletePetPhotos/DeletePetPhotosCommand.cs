namespace PetFamily.Application.Volunteers.UseCases.DeletePetPhotos;

public record DeletePetPhotosCommand(Guid VolunteerId, Guid PetId, List<string> PhotosNames);