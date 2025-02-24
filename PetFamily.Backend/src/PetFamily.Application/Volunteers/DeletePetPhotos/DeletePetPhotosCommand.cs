namespace PetFamily.Application.Volunteers.DeletePetPhotos;

public record DeletePetPhotosCommand(Guid VolunteerId, Guid PetId, List<string> PhotosNames);