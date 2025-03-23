using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.RefreshTokens;

public record RefreshTokensCommand(string AccessToken, Guid RefreshToken) : ICommand;