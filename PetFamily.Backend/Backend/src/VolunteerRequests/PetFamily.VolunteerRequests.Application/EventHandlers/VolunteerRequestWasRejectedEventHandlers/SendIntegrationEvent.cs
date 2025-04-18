using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasRejectedEventHandlers;

public class SendIntegrationEvent : INotificationHandler<VolunteerRequestWasRejectedEvent>
{
    private readonly ILogger<SendIntegrationEvent> _logger;
    private readonly IPublishEndpoint _publishEndpoint;


    public SendIntegrationEvent(
        ILogger<SendIntegrationEvent> logger,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(VolunteerRequestWasRejectedEvent domainEvent, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Contracts.Messaging.VolunteerRequestWasRejectedEvent(
            domainEvent.UserId,
            domainEvent.AdminId,
            domainEvent.RequestId), cancellationToken);
        
        _logger.LogInformation("Integration event \"VolunteerRequestWasRejectedEvent\" was published");
    }
}