using System.Text.Json;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Accounts.Infrastructure.Outbox;

namespace PetFamily.Accounts.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly AccountsDbContext _dbContext;

    public OutboxRepository(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add<T>(T message, CancellationToken cancellationToken)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOnUtc = DateTime.UtcNow,
            Type = typeof(T).FullName!,
            Payload = JsonSerializer.Serialize(message)
        };
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }
}