// ReSharper disable NotAccessedPositionalProperty.Global
namespace PetFamily.VolunteerRequests.Contracts.Messaging;

public record VolunteerRequestWasAmendedEvent(Guid UserId, Guid AdminId, Guid RequestId);