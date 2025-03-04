using FluentValidation;
using PetFamily.Application.Extensions;
using PetFamily.Domain.Shared.CustomErrors;

namespace PetFamily.Application.Volunteers.Commands.DeletePet;

public class DeletePetCommandValidator : AbstractValidator<DeletePetCommand>
{
    public DeletePetCommandValidator()
    {
        RuleFor(c => c.VolunteerId).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
        RuleFor(c => c.PetId).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
    }
}