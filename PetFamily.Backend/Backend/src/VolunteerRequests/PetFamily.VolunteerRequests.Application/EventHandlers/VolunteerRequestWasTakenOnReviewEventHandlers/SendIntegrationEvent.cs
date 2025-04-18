using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasTakenOnReviewEventHandlers;

public class SendIntegrationEvent : INotificationHandler<VolunteerRequestWasTakenOnReviewEvent>
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

    public async Task Handle(VolunteerRequestWasTakenOnReviewEvent domainEvent, CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(new Contracts.Messaging.VolunteerRequestWasTakenOnReviewEvent(
            domainEvent.UserId,
            domainEvent.AdminId,
            domainEvent.RequestId), cancellationToken);

        _logger.LogInformation("Integration event \"VolunteerRequestWasTakenOnReviewEvent\" was published");
    }
}