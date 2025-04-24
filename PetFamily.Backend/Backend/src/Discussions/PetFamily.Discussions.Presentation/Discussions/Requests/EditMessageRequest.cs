using PetFamily.Discussions.Application.Commands.EditMessage;

namespace PetFamily.Discussions.Presentation.Discussions.Requests;

public record EditMessageRequest(string NewMessageText)
{
    public EditMessageCommand ToCommand(Guid relationId, Guid messageId, Guid userId) =>
        new(relationId, messageId, userId, NewMessageText);
}