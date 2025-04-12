using FluentValidation;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Volunteers.Application.Commands.StartUploadPhotosToPet;

public class StartUploadPhotosToPetCommandValidator : AbstractValidator<StartUploadPhotosToPetCommand>
{
    public StartUploadPhotosToPetCommandValidator()
    {
        RuleFor(u => u.VolunteerId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(u => u.PetId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(u => u.FileInfos).SetValidator(new StartUploadFileDtoValidator());
    }
}

public class StartUploadFileDtoValidator : AbstractValidator<StartUploadFileDto>
{
    public StartUploadFileDtoValidator()
    {
        RuleFor(u => u.FileName).NotEmpty().WithError(Errors.General.ValueIsRequired());
        
        RuleFor(u => u.Size)
            .Must(c => c  < DomainConstants.MAX_FILE_SIZE_IN_BYTES)
            .WithError(Errors.General.ValueIsInvalid("fileSize"));
        
        RuleFor(u => u.ContentType)
            .Must(c => Photo.AllowedTypes.Contains(c))
            .WithError(Errors.General.ValueIsInvalid("contentType"));
    }
}