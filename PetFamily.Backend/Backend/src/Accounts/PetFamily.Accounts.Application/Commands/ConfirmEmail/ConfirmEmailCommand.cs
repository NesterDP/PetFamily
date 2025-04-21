using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(Guid UserId, string Token) : ICommand;