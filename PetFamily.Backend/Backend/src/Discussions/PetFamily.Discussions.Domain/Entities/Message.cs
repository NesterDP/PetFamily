using PetFamily.Discussions.Domain.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace PetFamily.Discussions.Domain.Entities;

public class Message
{
    public MessageId Id { get; private set; } = null!;

    public MessageText Text { get; private set; } = null!;

    public UserId UserId { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public IsEdited IsEdited { get; private set; } = null!;

    public Message(MessageText text, UserId userId)
    {
        Id = MessageId.NewMessageId();
        Text = text;
        UserId = userId;
        IsEdited = IsEdited.Create(false).Value;
    }

    // ReSharper disable once UnusedMember.Local
    private Message() { } // ef core

    public void Edit(MessageText text)
    {
        Text = text;
        IsEdited = IsEdited.Create(true).Value;
    }
}