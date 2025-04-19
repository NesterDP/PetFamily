using MassTransit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Structs;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasRejectedEventHandlers;

public class SendIntegrationEvent : INotificationHandler<VolunteerRequestWasRejectedEvent>
{
    private readonly ILogger<SendIntegrationEvent> _logger;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendIntegrationEvent(
        ILogger<SendIntegrationEvent> logger,
        IOutboxRepository outboxRepository,
        [FromKeyedServices(UnitOfWorkSelector.VolunteerRequests)]
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VolunteerRequestWasRejectedEvent domainEvent, CancellationToken cancellationToken)
    {
        var integrationEvent = new Contracts.Messaging.VolunteerRequestWasRejectedEvent(
            domainEvent.UserId,
            domainEvent.AdminId,
            domainEvent.RequestId);
        
        await _outboxRepository.Add(integrationEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Integration event \"VolunteerRequestWasRejectedEvent\" was saved in database");
    }
}