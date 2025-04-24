using PetFamily.Discussions.Application.Commands.AddMessage;

namespace PetFamily.Discussions.Presentation.Discussions.Requests;

public record AddMessageRequest(string MessageText)
{
    public AddMessageCommand ToCommand(Guid relationId, Guid userId) => new(relationId, userId, MessageText);
}