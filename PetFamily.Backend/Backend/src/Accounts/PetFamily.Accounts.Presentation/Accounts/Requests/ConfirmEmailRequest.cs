using PetFamily.Accounts.Application.Commands.ConfirmEmail;

namespace PetFamily.Accounts.Presentation.Accounts.Requests;

public record ConfirmEmailRequest(Guid UserId, string Token)
{
    public ConfirmEmailCommand ToCommand() => new(UserId, Token);
}