using System.Text.Json;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;
using PetFamily.VolunteerRequests.Infrastructure.Outbox;

namespace PetFamily.VolunteerRequests.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly WriteDbContext _dbContext;

    public OutboxRepository(WriteDbContext dbContext)
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