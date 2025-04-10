using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Volunteers.Application.Commands.UpdatePetMainPhoto;



public class UpdatePetMainPhotoValidator : AbstractValidator<UpdatePetMainPhotoCommand>
{
    public UpdatePetMainPhotoValidator()
    { 
        RuleFor(c => c.MainPhotoId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("MainPhotoId"));;
    }
}