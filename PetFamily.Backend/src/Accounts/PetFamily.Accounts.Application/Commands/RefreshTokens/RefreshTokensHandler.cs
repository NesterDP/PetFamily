using CSharpFunctionalExtensions;
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
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokensHandler(
        IRefreshSessionManager refreshSessionManager,
        ITokenProvider tokenProvider,
        [FromKeyedServices(UnitOfWorkSelector.Accounts)] IUnitOfWork unitOfWork)
    {
        _refreshSessionManager = refreshSessionManager;
        _tokenProvider = tokenProvider;
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

        var userIdString = userClaims.Value.FirstOrDefault(c => c.Type == CustomClaims.Id).Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Errors.General.ValueNotFound("user id").ToErrorList();
        
        if (oldRefreshSession.Value.UserId != userId)
            return Errors.General.InvalidToken().ToErrorList();
        
        var userJtiString = userClaims.Value.FirstOrDefault(c => c.Type == CustomClaims.Jti).Value;
        if (!Guid.TryParse(userJtiString, out var userJtiGuid))
            return Errors.General.ValueNotFound("user id").ToErrorList();
        
        if (oldRefreshSession.Value.Jti != userJtiGuid)
            return Errors.General.InvalidToken().ToErrorList();
        
        _refreshSessionManager.Delete(oldRefreshSession.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newAccessToken = await _tokenProvider
            .GenerateAccessToken(oldRefreshSession.Value.User, cancellationToken);
        
        var newRefreshToken = await _tokenProvider
            .GenerateRefreshToken(oldRefreshSession.Value.User, newAccessToken.Jti, cancellationToken);
        
        return new LoginResponse(newAccessToken.AccessToken, newRefreshToken);
    }
}