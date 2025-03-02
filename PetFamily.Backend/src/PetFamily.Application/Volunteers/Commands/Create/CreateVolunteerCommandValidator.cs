using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Commands.Create;

public class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
        RuleFor(c => c.VolunteerCommandDto.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Surname));
        RuleFor(c => c.VolunteerCommandDto.Email).MustBeValueObject(Email.Create);
        RuleFor(c => c.VolunteerCommandDto.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.VolunteerCommandDto.Experience).MustBeValueObject(Experience.Create);
        RuleFor(c => c.VolunteerCommandDto.PhoneNumber).MustBeValueObject(Phone.Create);
        
        RuleForEach(c => c.TransferDetailsDto)
            .MustBeValueObject(t => TransferDetail.Create(t.Name, t.Description));
        
        RuleForEach(c => c.SocialNetworksDto)
            .MustBeValueObject(s => SocialNetwork.Create(s.Name, s.Link));

        RuleFor(c => c.VolunteerCommandDto.Experience).Must(x => x >= 0 && x < 10)
            .WithError(Errors.General.ValueIsInvalid());
    }
}
    
