namespace PetFamily.Core.Dto.Discussion;

public class MessageDto
{
    public Guid Id { get; init; }

    public string Text { get; init; } = string.Empty;

    public Guid UserId { get; init; }

    public DateTime CreatedAt { get; init; }

    public bool IsEdited { get; init; }
}
