using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Domain.Entities;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;

namespace PetFamily.VolunteerRequests.Infrastructure.Repositories;

public class TestEntitiesRepository : ITestEntitiesRepository
{
    private readonly WriteDbContext _context;

    public TestEntitiesRepository(WriteDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<Guid> AddAsync(TestEntity testEntity, CancellationToken cancellationToken = default)
    {
        await _context.TestEntities.AddAsync(testEntity, cancellationToken);
        return testEntity.Id.Value;
    }
}