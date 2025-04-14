using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Volunteers.Application.Commands.CompleteUploadPhotosToPet;

public record CompleteUploadPhotosToPetCommand(
    Guid VolunteerId,
    Guid PetId,
    List<CompleteUploadFileDto> FileInfos) : ICommand;