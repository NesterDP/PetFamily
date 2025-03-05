using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Volunteers.Commands.DeletePetPhotos;

public record DeletePetPhotosCommand(Guid VolunteerId, Guid PetId, List<string> PhotosNames) : ICommand;