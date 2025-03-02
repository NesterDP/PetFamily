using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Volunteers.UseCases.UploadPhotosToPet;

public record UploadPhotosToPetCommand(Guid VolunteerId, Guid PetId, IEnumerable<UploadFileDto> Files);