using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Volunteers.Application.Commands.StartUploadPhotosToPet;

public record StartUploadPhotosToPetCommand(
    Guid VolunteerId,
    Guid PetId,
    List<StartUploadFileDto> FileInfos) : ICommand;