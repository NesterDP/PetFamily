using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.VolunteerRequests.Domain.Entities;

public class TestEntity : DomainEntity<TestId>
{
    public bool Status { get; private set; }

    public UserId UserId { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TestEntity(TestId id)
        : base(id)
    {
    }

    public TestEntity(TestId id, UserId userId)
        : base(id)
    {
        UserId = userId;
    }

    public void SetStatus(bool status) => Status = status;
}