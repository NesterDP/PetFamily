using FluentValidation;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.Extensions;
using PetFamily.Core.SharedVO;
using PetFamily.Volunteers.Domain.ValueObjects.VolunteerVO;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdateMainInfo;

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