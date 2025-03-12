using PetFamily.Core.Abstractions;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.DeletePetPhotos;

public record DeletePetPhotosCommand(Guid VolunteerId, Guid PetId, List<string> PhotosNames) : ICommand;