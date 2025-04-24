using FluentValidation;
using PetFamily.Core.Extensions;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Commands.ConfirmEmail;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty().WithError(Errors.General.ValueIsRequired());
        RuleFor(c => c.Token).NotEmpty().WithError(Errors.General.ValueIsRequired());
    }
}