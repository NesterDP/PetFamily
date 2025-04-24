using PetFamily.Accounts.Application.Commands.Register;

namespace PetFamily.Accounts.Presentation.Accounts.Requests;

public record RegisterUserRequest(string Email, string UserName, string Password)
{
    public RegisterUserCommand ToCommand() => new(Email, UserName, Password);
}