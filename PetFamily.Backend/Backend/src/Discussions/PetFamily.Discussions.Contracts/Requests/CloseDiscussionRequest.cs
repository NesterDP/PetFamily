namespace PetFamily.Discussions.Contracts.Requests;

public record CloseDiscussionRequest(Guid RelationId, Guid UserId);