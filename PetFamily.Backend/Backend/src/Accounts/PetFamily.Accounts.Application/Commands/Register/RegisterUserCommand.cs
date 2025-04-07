using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.Register;

public record RegisterUserCommand(string Email, string UserName, string Password) : ICommand;