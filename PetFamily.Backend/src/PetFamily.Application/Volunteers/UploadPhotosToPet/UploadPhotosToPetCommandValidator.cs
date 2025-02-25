using FluentValidation;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UploadPhotosToPet;

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
            .Must(c => c.Length < Constants.MAX_FILE_SIZE_IN_BYTES)
            .WithError(Errors.General.ValueIsInvalid("fileSize"));
    }
}