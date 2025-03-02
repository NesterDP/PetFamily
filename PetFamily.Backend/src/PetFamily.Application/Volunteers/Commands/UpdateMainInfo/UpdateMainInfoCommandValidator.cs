using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.Commands.UpdateMainInfo;

public class UpdateMainInfoCommandValidator : AbstractValidator<UpdateMainInfoCommand>
{
    public UpdateMainInfoCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(r => r.FullNameDto)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Surname));
        RuleFor(r => r.Email).MustBeValueObject(Email.Create);
        RuleFor(r => r.Description).MustBeValueObject(Description.Create);
        RuleFor(r => r.Experience).MustBeValueObject(Experience.Create);
        RuleFor(r => r.PhoneNumber).MustBeValueObject(Phone.Create);
    }
    
} 