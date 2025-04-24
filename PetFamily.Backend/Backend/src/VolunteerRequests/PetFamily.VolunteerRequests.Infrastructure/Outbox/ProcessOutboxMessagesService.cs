using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.VolunteerRequests.Contracts.StaticClasses;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;
using Polly;
using Polly.Retry;

namespace PetFamily.VolunteerRequests.Infrastructure.Outbox;

public class ProcessOutboxMessagesService
{
    private const int HANDLED_MESSAGES_AT_ONCE = 20;
    private readonly IPublishEndpoint _publisher;
    private readonly WriteDbContext _dbContext;
    private readonly ILogger<ProcessOutboxMessagesService> _logger;

    public ProcessOutboxMessagesService(
        IPublishEndpoint publisher,
        WriteDbContext writeDbContext,
        ILogger<ProcessOutboxMessagesService> logger)
    {
        _publisher = publisher;
        _dbContext = writeDbContext;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var messages = await _dbContext
            .Set<OutboxMessage>()
            .OrderBy(m => m.OccurredOnUtc)
            .Where(m => m.ProcessedOnUtc == null)
            .Take(HANDLED_MESSAGES_AT_ONCE)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
            return;

        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(
                new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(1),
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                    OnRetry = retryArguments =>
                    {
                        _logger.LogCritical(
                            retryArguments.Outcome.Exception,
                            "Current attempt: {attemptNumber}",
                            retryArguments.AttemptNumber);

                        return ValueTask.CompletedTask;
                    }
                })
            .Build();

        var processingTasks = messages.Select(
            message =>
                ProcessMessageAsync(message, pipeline, cancellationToken));

        await Task.WhenAll(processingTasks);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to save changes to the database");
        }
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message,
        ResiliencePipeline pipeline,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var messageType = AssemblyReference.Assembly.GetType(message.Type)
                              ?? throw new NullReferenceException("Message type not found");

            object deserializedMessage = JsonSerializer.Deserialize(message.Payload, messageType)
                                         ?? throw new NullReferenceException("Message payload is not found");

            await pipeline.ExecuteAsync(
                async token =>
                {
                    await _publisher.Publish(deserializedMessage, messageType, token);

                    message.ProcessedOnUtc = DateTime.UtcNow;
                }, cancellationToken);
        }
        catch (Exception e)
        {
            message.Error = e.Message;
            message.ProcessedOnUtc = DateTime.UtcNow;
            _logger.LogError(e, "Failed to process message ID: {MessageId}", message.Id);
        }
    }
}