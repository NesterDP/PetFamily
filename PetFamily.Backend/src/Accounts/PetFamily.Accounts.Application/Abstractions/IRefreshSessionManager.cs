using CSharpFunctionalExtensions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Application.Abstractions;

public interface IRefreshSessionManager
{
    public Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken,
        CancellationToken cancellationToken);

    void Delete(RefreshSession refreshSession);
}