using MediatR;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestSentForApprovedEventHandlers;

public class TestEntityCreationWithTrueStatus : INotificationHandler<VolunteerRequestSentForApprovedEvent>
{
    private readonly ITestEntitiesRepository _repository;

    public TestEntityCreationWithTrueStatus(ITestEntitiesRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(VolunteerRequestSentForApprovedEvent domainEvent, CancellationToken cancellationToken)
    {
        var testEntity = new TestEntity(TestId.NewTestId(), domainEvent.UserId);
        testEntity.SetStatus(true);

        await _repository.AddAsync(testEntity, cancellationToken);
    }
}