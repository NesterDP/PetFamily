// ReSharper disable NotAccessedPositionalProperty.Global

namespace PetFamily.VolunteerRequests.Contracts.Messaging;

public record VolunteerRequestRequiredRevisionEvent(Guid UserId, Guid AdminId, Guid RequestId);