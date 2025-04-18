namespace PetFamily.VolunteerRequests.Contracts.Messaging;

public record VolunteerRequestWasRejectedEvent(Guid UserId, Guid AdminId, Guid RequestId);