namespace PetFamily.VolunteerRequests.Contracts.Messaging;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record VolunteerRequestWasRejectedEvent(Guid UserId, Guid AdminId, Guid RequestId);