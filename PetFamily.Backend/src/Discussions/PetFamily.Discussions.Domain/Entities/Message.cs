using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Domain.Entities;


public class Message
{
    public MessageId Id { get; private set; }
    public MessageText Text { get; private set; }
    public UserId UserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public IsEdited IsEdited { get; private set; }
    
    private Message() { } // ef core

    public Message(MessageText text, UserId userId)
    {
        Id = MessageId.NewMessageId();
        Text = text;
        UserId = userId;
        IsEdited = IsEdited.Create(false).Value;
    }

    public void Edit(MessageText text)
    {
        Text = text;
        IsEdited = IsEdited.Create(true).Value;
    }
}