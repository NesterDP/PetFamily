using PetFamily.Core.Abstractions;

namespace PetFamily.Discussions.Application.Queries.GetDiscussion;

public record GetDiscussionQuery(Guid RelationId) : IQuery;