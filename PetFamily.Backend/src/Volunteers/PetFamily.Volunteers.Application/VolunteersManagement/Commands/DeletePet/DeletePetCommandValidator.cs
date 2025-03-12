using FluentValidation;
using PetFamily.Core.CustomErrors;
using PetFamily.Core.Extensions;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.DeletePet;

public class DeletePetCommandValidator : AbstractValidator<DeletePetCommand>
{
    public DeletePetCommandValidator()
    {
        RuleFor(c => c.VolunteerId).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
        RuleFor(c => c.PetId).NotEmpty().WithError(Errors.General.ValueIsRequired()); 
    }
}