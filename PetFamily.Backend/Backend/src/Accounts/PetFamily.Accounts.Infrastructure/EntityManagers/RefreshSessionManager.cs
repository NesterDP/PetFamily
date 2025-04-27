using CSharpFunctionalExtensions;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Caching;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Infrastructure.EntityManagers;

public class RefreshSessionManager : IRefreshSessionManager
{
    private readonly ICacheService _cacheService;

    public RefreshSessionManager(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken,
        CancellationToken cancellationToken)
    {
        string key = CacheConstants.REFRESH_SESSIONS_PREFIX + refreshToken;
        var session = await _cacheService.GetAsync<RefreshSession>(key, cancellationToken);

        return session ?? Result.Failure<RefreshSession, Error>(Errors.General.ValueNotFound(refreshToken));
    }

    public async Task<Guid> DeleteAsync(RefreshSession refreshSession, CancellationToken cancellationToken)
    {
        string key = CacheConstants.REFRESH_SESSIONS_PREFIX + refreshSession.RefreshToken;
        await _cacheService.RemoveAsync(key, cancellationToken);
        return refreshSession.RefreshToken;
    }
}