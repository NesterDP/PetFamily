using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Accounts.Application.Commands.CompleteUploadAvatar;

public record CompleteUploadAvatarCommand(Guid UserId, CompleteUploadFileDto FileInfo) : ICommand;