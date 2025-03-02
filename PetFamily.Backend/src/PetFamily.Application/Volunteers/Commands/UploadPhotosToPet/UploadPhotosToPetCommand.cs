using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.Commands.UploadPhotosToPet;

public record UploadPhotosToPetCommand(Guid VolunteerId, Guid PetId, IEnumerable<UploadFileDto> Files);