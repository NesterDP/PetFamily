using FluentValidation;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.Extensions;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.Delete;

public class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
    }
}