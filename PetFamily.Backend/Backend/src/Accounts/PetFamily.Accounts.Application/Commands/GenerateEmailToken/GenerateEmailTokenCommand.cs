using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.GenerateEmailToken;

public record GenerateEmailTokenCommand(Guid UserId) : ICommand;