using PetFamily.Core.Abstractions;

namespace PetFamily.Discussions.Application.Commands.CloseDiscussion;

public record CloseDiscussionCommand(Guid RelationId, Guid UserId) : ICommand;