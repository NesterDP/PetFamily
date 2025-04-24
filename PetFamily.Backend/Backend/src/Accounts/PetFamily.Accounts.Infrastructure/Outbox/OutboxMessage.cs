namespace PetFamily.Accounts.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }

    public required string Type { get; init; } = string.Empty;

    public required string Payload { get; init; } = string.Empty;

    public required DateTime OccurredOnUtc { get; init; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
}