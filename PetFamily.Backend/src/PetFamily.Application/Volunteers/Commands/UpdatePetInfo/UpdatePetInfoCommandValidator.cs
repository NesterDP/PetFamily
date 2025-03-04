using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.PetVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Commands.UpdatePetInfo;

public class UpdatePetInfoCommandValidator : AbstractValidator<UpdatePetInfoCommand>
{
    public UpdatePetInfoCommandValidator()
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
        
        RuleForEach(c => c.TransferDetailsDto)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
    }
}