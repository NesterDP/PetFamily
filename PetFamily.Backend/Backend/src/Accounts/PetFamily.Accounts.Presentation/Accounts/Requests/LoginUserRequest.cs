using PetFamily.Accounts.Application.Commands.Login;

namespace PetFamily.Accounts.Presentation.Accounts.Requests;

public record LoginUserRequest(string Email, string Password)
{
    public LoginUserCommand ToCommand() => new LoginUserCommand(Email, Password);
}