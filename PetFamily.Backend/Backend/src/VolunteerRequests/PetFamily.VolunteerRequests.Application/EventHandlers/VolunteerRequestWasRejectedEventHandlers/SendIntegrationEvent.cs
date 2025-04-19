using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasRejectedEventHandlers;

public class SendIntegrationEvent : INotificationHandler<VolunteerRequestWasRejectedEvent>
{
    private readonly ILogger<SendIntegrationEvent> _logger;
    private readonly IOutboxRepository _outboxRepository;

    public SendIntegrationEvent(
        ILogger<SendIntegrationEvent> logger,
        IOutboxRepository outboxRepository)
    {
        _logger = logger;
        _outboxRepository = outboxRepository;
    }

    public async Task Handle(VolunteerRequestWasRejectedEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new Contracts.Messaging.VolunteerRequestWasRejectedEvent(
            domainEvent.UserId,
            domainEvent.AdminId,
            domainEvent.RequestId);
        
        await _outboxRepository.Add(integrationEvent, cancellationToken);
        
        _logger.LogInformation("Integration event \"VolunteerRequestWasRejectedEvent\" was saved in database");
    }
}