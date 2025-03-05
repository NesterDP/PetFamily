using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;

public record UpdatePetMainPhotoCommand(Guid VolunteerId, Guid PetId, string MainPhotoPath) : ICommand;
