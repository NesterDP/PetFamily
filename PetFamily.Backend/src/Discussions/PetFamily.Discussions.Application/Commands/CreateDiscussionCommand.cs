using PetFamily.Core.Abstractions;

namespace PetFamily.Discussions.Application.Commands;

public record CreateDiscussionCommand(Guid RelationId, List<Guid> UserIds) : ICommand;
