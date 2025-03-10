using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Commands.UpdatePetMainPhoto;



public class UpdatePetMainPhotoValidator : AbstractValidator<UpdatePetMainPhotoCommand>
{
    public UpdatePetMainPhotoValidator()
    { 
        RuleFor(c => c.MainPhotoPath).MustBeValueObject(FilePath.Create);
    }
}