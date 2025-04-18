using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasApprovedEventHandlers;

public class SendIntegrationEvent : INotificationHandler<VolunteerRequestWasApprovedEvent>
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

    public async Task Handle(VolunteerRequestWasApprovedEvent domainEvent, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Contracts.Messaging.VolunteerRequestWasApprovedEvent(
            domainEvent.UserId,
            domainEvent.AdminId,
            domainEvent.RequestId), cancellationToken);
        
        _logger.LogInformation("Integration event \"VolunteerRequestWasApprovedEvent\" was published");
    }
}