using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.VolunteerRequests.Domain.Events;

public record VolunteerRequestWasRejectedEvent(UserId UserId) : IDomainEvent;