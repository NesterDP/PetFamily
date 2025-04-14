using Microsoft.AspNetCore.Http;
using PetFamily.Core.Dto.Shared;
using PetFamily.Volunteers.Application.Commands.StartUploadPhotosToPet;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record StartUploadPhotosToPetRequest(List<StartUploadFileDto> FileInfos)
{
    public StartUploadPhotosToPetCommand ToCommand(Guid volunteerId, Guid petId) => new(volunteerId, petId, FileInfos);
}