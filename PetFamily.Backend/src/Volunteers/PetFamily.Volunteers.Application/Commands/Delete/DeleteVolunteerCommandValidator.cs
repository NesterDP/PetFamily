using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Volunteers.Application.Commands.Delete;

public class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
    }
}