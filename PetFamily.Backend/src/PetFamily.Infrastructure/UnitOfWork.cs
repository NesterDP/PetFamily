using PetFamily.Infrastructure.DbContexts;

namespace PetFamily.Infrastructure;

using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using PetFamily.Application.Database;



public class UnitOfWork : IUnitOfWork
{
    private readonly WriteDbContext _dbContext;

    public UnitOfWork(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}