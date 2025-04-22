using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.GenerateEmailToken;

public class GenerateEmailTokenCommandValidator : AbstractValidator<GenerateEmailTokenCommand>
{
    public GenerateEmailTokenCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}