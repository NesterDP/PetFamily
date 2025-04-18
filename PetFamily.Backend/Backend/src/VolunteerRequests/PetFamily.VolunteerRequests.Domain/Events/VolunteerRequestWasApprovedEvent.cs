using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.VolunteerRequests.Domain.Events;

public record VolunteerRequestWasApprovedEvent(
    UserId UserId,
    AdminId AdminId,
    VolunteerRequestId RequestId) : IDomainEvent;