namespace PetFamily.VolunteerRequests.Contracts.Messaging;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record VolunteerRequestWasApprovedEvent(Guid UserId, Guid AdminId, Guid RequestId);