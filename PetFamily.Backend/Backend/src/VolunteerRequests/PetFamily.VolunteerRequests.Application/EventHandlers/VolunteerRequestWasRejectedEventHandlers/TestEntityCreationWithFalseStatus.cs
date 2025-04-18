using MediatR;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasRejectedEventHandlers;


public class TestEntityCreationWithFalseStatus : INotificationHandler<VolunteerRequestWasRejectedEvent>
{
    private readonly ITestEntitiesRepository _repository;

    public TestEntityCreationWithFalseStatus(ITestEntitiesRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(VolunteerRequestWasRejectedEvent domainEvent, CancellationToken cancellationToken)
    {
        var testEntity = new TestEntity(TestId.NewTestId(), domainEvent.UserId);
        testEntity.SetStatus(false);

        await _repository.AddAsync(testEntity, cancellationToken);
    }
}