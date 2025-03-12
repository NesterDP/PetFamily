using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.ValueObjects.PetVO;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.AddPet;

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
            .MustBeValueObject(Color.Create);
        
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