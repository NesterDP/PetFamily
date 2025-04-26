using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Contracts.Responses;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.Structs;

namespace PetFamily.Accounts.Application.Commands.RefreshTokens;

public class RefreshTokensHandler : ICommandHandler<LoginResponse, RefreshTokensCommand>
{
    private readonly IRefreshSessionManager _refreshSessionManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokensHandler(
        IRefreshSessionManager refreshSessionManager,
        ITokenProvider tokenProvider,
        UserManager<User> userManager,
        [FromKeyedServices(UnitOfWorkSelector.Accounts)]
        IUnitOfWork unitOfWork)
    {
        _refreshSessionManager = refreshSessionManager;
        _tokenProvider = tokenProvider;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse, ErrorList>> HandleAsync(
        RefreshTokensCommand command,
        CancellationToken cancellationToken = default)
    {
        var oldRefreshSession = await _refreshSessionManager
            .GetByRefreshToken(command.RefreshToken, cancellationToken);

        if (oldRefreshSession.IsFailure)
            return oldRefreshSession.Error.ToErrorList();

        if (DateTime.UtcNow > oldRefreshSession.Value.ExpiresIn)
            return Errors.General.ExpiredToken().ToErrorList();

        var userClaims = await _tokenProvider.GetUserClaims(command.AccessToken);
        if (userClaims.IsFailure)
            return userClaims.Error.ToErrorList();

        string userIdString = userClaims.Value.FirstOrDefault(c => c.Type == CustomClaims.ID)!.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Errors.General.ValueNotFound("user id").ToErrorList();

        if (oldRefreshSession.Value.UserId != userId)
            return Errors.General.InvalidToken().ToErrorList();

        string userJtiString = userClaims.Value.FirstOrDefault(c => c.Type == CustomClaims.JTI)!.Value;
        if (!Guid.TryParse(userJtiString, out var userJtiGuid))
            return Errors.General.ValueNotFound("user jti").ToErrorList();

        if (oldRefreshSession.Value.Jti != userJtiGuid)
            return Errors.General.InvalidToken().ToErrorList();

        _refreshSessionManager.Delete(oldRefreshSession.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var user = await _userManager.FindByIdAsync(userIdString);

        var newAccessToken = await _tokenProvider
            .GenerateAccessToken(user!, cancellationToken);

        var newRefreshToken = await _tokenProvider
            .GenerateRefreshToken(user!, newAccessToken.Jti, cancellationToken);

        return new LoginResponse(newAccessToken.AccessToken, newRefreshToken);
    }
}