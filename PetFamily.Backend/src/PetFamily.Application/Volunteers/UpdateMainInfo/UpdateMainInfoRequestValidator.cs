using System.Data;
using FluentValidation;
using PetFamily.API.Extensions;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.CustomErrors;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoRequestValidator : AbstractValidator<UpdateMainInfoRequest>
{
    public UpdateMainInfoRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(r => r.Dto.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.LastName, f.Surname));
        RuleFor(r => r.Dto.Email).MustBeValueObject(Email.Create);
        RuleFor(r => r.Dto.Description).MustBeValueObject(Description.Create);
        RuleFor(r => r.Dto.Experience).MustBeValueObject(Experience.Create);
        RuleFor(r => r.Dto.PhoneNumber).MustBeValueObject(Phone.Create);
    }
    
} 