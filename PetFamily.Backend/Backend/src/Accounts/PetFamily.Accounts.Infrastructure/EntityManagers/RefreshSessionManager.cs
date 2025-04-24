using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Infrastructure.EntityManagers;

public class RefreshSessionManager : IRefreshSessionManager
{
    private readonly AccountsDbContext _accountsDbContext;

    public RefreshSessionManager(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken,
        CancellationToken cancellationToken)
    {
        var refreshSession = await _accountsDbContext.RefreshSessions
            .Include(rs => rs.User)
            .ThenInclude(u => u.Roles)
            .FirstOrDefaultAsync(rs => rs.RefreshToken == refreshToken, cancellationToken);

        if (refreshSession is null)
            return Result.Failure<RefreshSession, Error>(Errors.General.ValueNotFound(refreshToken));

        return refreshSession;
    }

    public void Delete(RefreshSession refreshSession)
    {
        _accountsDbContext.RefreshSessions.Remove(refreshSession);
    }
}