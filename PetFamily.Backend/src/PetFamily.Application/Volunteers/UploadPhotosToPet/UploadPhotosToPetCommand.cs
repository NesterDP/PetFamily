using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.UploadPhotosToPet;

public record UploadPhotosToPetCommand(Guid VolunteerId, Guid PetId, IEnumerable<UploadFileDto> Files);