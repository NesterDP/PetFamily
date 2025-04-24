using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Application.Commands.CompleteUploadPhotosToPet;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record CompleteUploadPhotosToPetRequest(List<CompleteUploadFileDto> FileInfos)
{
    public CompleteUploadPhotosToPetCommand ToCommand(Guid volunteerId, Guid petId) =>
        new(volunteerId, petId, FileInfos);
}