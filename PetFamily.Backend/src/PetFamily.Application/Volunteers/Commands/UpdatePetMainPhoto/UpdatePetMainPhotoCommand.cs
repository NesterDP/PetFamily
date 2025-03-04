namespace PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;

public record UpdatePetMainPhotoCommand(Guid VolunteerId, Guid PetId, string MainPhotoPath);
