using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Structs;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Domain.Events;

namespace PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasRejectedEventHandlers;

public class TestEntityCreationWithFalseStatus : INotificationHandler<VolunteerRequestWasRejectedEvent>
{
    private readonly ITestEntitiesRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TestEntityCreationWithFalseStatus(
        ITestEntitiesRepository repository,
        [FromKeyedServices(UnitOfWorkSelector.VolunteerRequests)]
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VolunteerRequestWasRejectedEvent domainEvent, CancellationToken cancellationToken)
    {
        var testEntity = new TestEntity(TestId.NewTestId(), domainEvent.UserId);
        testEntity.SetStatus(false);

        await _repository.AddAsync(testEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}