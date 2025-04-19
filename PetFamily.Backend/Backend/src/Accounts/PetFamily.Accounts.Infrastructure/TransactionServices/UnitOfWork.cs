using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Core.Abstractions;

namespace PetFamily.Accounts.Infrastructure.TransactionServices;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountsDbContext _dbContext;

    public UnitOfWork(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}