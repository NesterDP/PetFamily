using FluentValidation;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Application.Commands.CompleteUploadPhotosToPet;

public class CompleteUploadPhotosToPetCommandValidator : AbstractValidator<CompleteUploadPhotosToPetCommand>
{
    public CompleteUploadPhotosToPetCommandValidator()
    {
        RuleFor(u => u.VolunteerId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(u => u.PetId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleForEach(u => u.FileInfos).SetValidator(new CompleteUploadFileDtoValidator());
    }
}

public class CompleteUploadFileDtoValidator : AbstractValidator<CompleteUploadFileDto>
{
    public CompleteUploadFileDtoValidator()
    {
        RuleFor(u => u.Key).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(u => u.UploadId).NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}