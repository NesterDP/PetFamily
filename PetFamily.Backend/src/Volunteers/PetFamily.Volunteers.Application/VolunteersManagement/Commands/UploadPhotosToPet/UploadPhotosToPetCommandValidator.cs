using FluentValidation;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Extensions;
using PetFamily.Core.GeneralClasses;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UploadPhotosToPet;

public class UploadPhotosToPetCommandValidator : AbstractValidator<UploadPhotosToPetCommand>
{
    public UploadPhotosToPetCommandValidator()
    {
        RuleFor(u => u.VolunteerId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(u => u.PetId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(u => u.Files).SetValidator(new UploadFileDtoValidator());
    }
}

public class UploadFileDtoValidator : AbstractValidator<UploadFileDto>
{
    public UploadFileDtoValidator()
    {
        RuleFor(u => u.FileName).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(u => u.Content)
            .Must(c => c.Length < DomainConstants.MAX_FILE_SIZE_IN_BYTES)
            .WithError(Errors.General.ValueIsInvalid("fileSize"));
    }
}