using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Accounts.Domain.Events;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record UserWasRegisteredEvent(UserId UserId) : IDomainEvent;