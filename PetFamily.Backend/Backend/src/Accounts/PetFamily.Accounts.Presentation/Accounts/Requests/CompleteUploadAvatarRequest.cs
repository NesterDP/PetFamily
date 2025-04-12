using PetFamily.Accounts.Application.Commands.CompleteUploadAvatar;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Accounts.Presentation.Accounts.Requests;

public record CompleteUploadAvatarRequest(CompleteUploadFileDto FileInfo)
{
    public CompleteUploadAvatarCommand ToCommand(Guid userId) => new(userId, FileInfo);
}