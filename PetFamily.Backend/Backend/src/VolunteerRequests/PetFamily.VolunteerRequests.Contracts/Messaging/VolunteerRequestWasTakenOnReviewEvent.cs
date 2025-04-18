namespace PetFamily.VolunteerRequests.Contracts.Messaging;

public record VolunteerRequestWasTakenOnReviewEvent(Guid UserId, Guid AdminId, Guid RequestId);