using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.UseCases.Delete;

public class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
    }
}