using Microsoft.EntityFrameworkCore.Storage;

namespace PetFamily.Core.Abstractions;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}