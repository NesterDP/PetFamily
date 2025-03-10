using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Create;

public class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
        RuleFor(c => c.VolunteerDto.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Surname));
        RuleFor(c => c.VolunteerDto.Email).MustBeValueObject(Email.Create);
        RuleFor(c => c.VolunteerDto.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.VolunteerDto.Experience).MustBeValueObject(Experience.Create);
        RuleFor(c => c.VolunteerDto.PhoneNumber).MustBeValueObject(Phone.Create);
        
        RuleForEach(c => c.TransferDetailsDto)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
        
        RuleForEach(c => c.SocialNetworksDto)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Link));

        RuleFor(c => c.VolunteerDto.Experience).Must(x => x >= 0 && x < 10)
            .WithError(Errors.General.ValueIsInvalid());
    }
}
    
