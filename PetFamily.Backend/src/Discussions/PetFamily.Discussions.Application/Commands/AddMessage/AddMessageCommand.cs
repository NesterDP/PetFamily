using PetFamily.Core.Abstractions;

namespace PetFamily.Discussions.Application.Commands.AddMessage;

public record AddMessageCommand(Guid RelationId, Guid UserId, string MessageText) : ICommand;