using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.CompleteUploadAvatar;

public class CompleteUploadAvatarCommandValidator : AbstractValidator<CompleteUploadAvatarCommand>
{
    public CompleteUploadAvatarCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(c => c.FileInfo.Key).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(c => c.FileInfo.UploadId).NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}