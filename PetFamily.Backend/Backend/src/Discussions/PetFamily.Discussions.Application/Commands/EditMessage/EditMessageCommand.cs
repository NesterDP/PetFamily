using PetFamily.Core.Abstractions;

namespace PetFamily.Discussions.Application.Commands.EditMessage;

public record EditMessageCommand(Guid RelationId, Guid MessageId, Guid UserId, string NewMessageText) : ICommand;