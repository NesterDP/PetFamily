using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.StartUploadAvatar;

public class StartUploadAvatarCommandValidator : AbstractValidator<StartUploadAvatarCommand>
{
    public StartUploadAvatarCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(c => c.FileInfo.FileName).NotEmpty().WithError(Errors.General.ValueIsRequired());

        RuleFor(c => c.FileInfo.ContentType)
            .Must(c => Avatar.AllowedTypes.Contains(c))
            .WithError(Errors.General.ValueIsInvalid("contentType"));

        RuleFor(c => c.FileInfo.Size)
            .Must(c => c < DomainConstants.MAX_FILE_SIZE_IN_BYTES)
            .WithError(Errors.General.ValueIsInvalid("fileSize"));
    }
}