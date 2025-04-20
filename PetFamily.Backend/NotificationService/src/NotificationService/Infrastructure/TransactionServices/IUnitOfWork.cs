using Microsoft.EntityFrameworkCore.Storage;

namespace NotificationService.Infrastructure.TransactionServices;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}