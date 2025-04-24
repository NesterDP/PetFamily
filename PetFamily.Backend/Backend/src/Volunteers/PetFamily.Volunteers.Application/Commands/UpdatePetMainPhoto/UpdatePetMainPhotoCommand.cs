using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.Commands.UpdatePetMainPhoto;

public record UpdatePetMainPhotoCommand(Guid VolunteerId, Guid PetId, Guid MainPhotoId) : ICommand;