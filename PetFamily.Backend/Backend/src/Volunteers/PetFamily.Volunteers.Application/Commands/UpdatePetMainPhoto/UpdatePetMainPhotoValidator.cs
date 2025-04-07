using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Volunteers.Application.Commands.UpdatePetMainPhoto;



public class UpdatePetMainPhotoValidator : AbstractValidator<UpdatePetMainPhotoCommand>
{
    public UpdatePetMainPhotoValidator()
    { 
        RuleFor(c => c.MainPhotoPath).MustBeValueObject(FilePath.Create);
    }
}