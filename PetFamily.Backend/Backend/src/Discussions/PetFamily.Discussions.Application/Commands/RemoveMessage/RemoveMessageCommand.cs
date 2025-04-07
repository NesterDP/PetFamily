using PetFamily.Core.Abstractions;

namespace PetFamily.Discussions.Application.Commands.RemoveMessage;

public record RemoveMessageCommand(Guid RelationId, Guid MessageId, Guid UserId) : ICommand;