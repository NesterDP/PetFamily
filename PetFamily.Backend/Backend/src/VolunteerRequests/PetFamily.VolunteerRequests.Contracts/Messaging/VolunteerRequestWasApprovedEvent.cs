namespace PetFamily.VolunteerRequests.Contracts.Messaging;

public record VolunteerRequestWasApprovedEvent(Guid UserId, Guid AdminId, Guid RequestId);