using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Application.Abstractions;

public interface ITestEntitiesRepository
{
    Task<Guid> AddAsync(TestEntity testEntity, CancellationToken cancellationToken = default);
}