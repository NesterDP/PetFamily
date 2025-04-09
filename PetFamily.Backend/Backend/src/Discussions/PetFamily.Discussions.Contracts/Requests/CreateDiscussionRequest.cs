namespace PetFamily.Discussions.Contracts.Requests;

public record CreateDiscussionRequest(Guid RelationId, List<Guid> UserIds);