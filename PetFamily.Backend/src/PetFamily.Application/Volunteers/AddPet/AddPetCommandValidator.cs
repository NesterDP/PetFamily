using System.Drawing;
using FluentValidation;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetCommandValidator : AbstractValidator<AddPetCommand>
{
    public AddPetCommandValidator()
    {
        
        RuleFor(c => c.Name).MustBeValueObject(Name.Create);
        RuleFor(c => c.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.PetClassificationDto.SpeciesId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("speciesId"));
        RuleFor(c => c.PetClassificationDto.BreedId)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("breedId"));
        
        RuleFor(c => c.Color)
            .MustBeValueObject(PetFamily.Domain.PetContext.ValueObjects.PetVO.Color.Create);
        
        RuleFor(c => c.HealthInfo).MustBeValueObject(HealthInfo.Create);
        
        RuleFor(c => c.AddressDto)
            .MustBeValueObject(a => Address.Create(a.City, a.House, a.Apartment));
        RuleFor(c => c.Weight).MustBeValueObject(Weight.Create);
        RuleFor(c => c.Height).MustBeValueObject(Height.Create);
        RuleFor(c => c.OwnerPhoneNumber).MustBeValueObject(Phone.Create);
        RuleFor(c => c.IsCastrated).MustBeValueObject(IsCastrated.Create);
        RuleFor(c => c.DateOfBirth).MustBeValueObject(DateOfBirth.Create);
        RuleFor(c => c.IsVaccinated).MustBeValueObject(IsVaccinated.Create);
        RuleFor(c => c.HelpStatus).MustBeValueObject(HelpStatus.Create);
        
        RuleForEach(c => c.TransferDetailsDto)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
    }
}