using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Application.Commands.Create;

public class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
        RuleFor(c => c.CreateVolunteerDto.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Surname));
        RuleFor(c => c.CreateVolunteerDto.Email).MustBeValueObject(Email.Create);
        RuleFor(c => c.CreateVolunteerDto.Description).MustBeValueObject(Description.Create);
        RuleFor(c => c.CreateVolunteerDto.Experience).MustBeValueObject(Experience.Create);
        RuleFor(c => c.CreateVolunteerDto.PhoneNumber).MustBeValueObject(Phone.Create);
        
        /*RuleFor(c => c.VolunteerCommandDto.Experience).Must(x => x >= 0 && x < 10)
            .WithError(Errors.General.ValueIsInvalid());*/
    }
}
    
