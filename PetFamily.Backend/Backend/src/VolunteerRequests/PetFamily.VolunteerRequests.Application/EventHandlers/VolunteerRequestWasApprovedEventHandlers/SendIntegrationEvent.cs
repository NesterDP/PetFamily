using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasApprovedEventHandlers;

public class SendIntegrationEvent : INotificationHandler<VolunteerRequestWasApprovedEvent>
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

    public async Task Handle(VolunteerRequestWasApprovedEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new Contracts.Messaging.VolunteerRequestWasApprovedEvent(
            domainEvent.UserId,
            domainEvent.AdminId,
            domainEvent.RequestId);
        
        await _outboxRepository.Add(integrationEvent, cancellationToken);
        
        _logger.LogInformation("Integration event \"VolunteerRequestWasApprovedEvent\" was saved in database");
    }
}