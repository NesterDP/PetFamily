using PetFamily.Accounts.Application.Commands.StartUploadAvatar;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Accounts.Presentation.Accounts.Requests;

public record StartUploadAvatarRequest(StartUploadFileDto FileInfo)
{
    public StartUploadAvatarCommand ToCommand(Guid userId) => new(userId, FileInfo);
}