using FluentValidation;
using PetFamily.API.Extensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.Delete;

public class DeleteVolunteerRequestValidator : AbstractValidator<DeleteVolunteerRequest>
{
    public DeleteVolunteerRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
    }
}