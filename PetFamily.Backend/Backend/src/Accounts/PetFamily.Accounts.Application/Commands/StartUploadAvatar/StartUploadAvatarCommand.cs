using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Accounts.Application.Commands.StartUploadAvatar;

public record StartUploadAvatarCommand(Guid UserId, StartUploadFileDto FileInfo) : ICommand;