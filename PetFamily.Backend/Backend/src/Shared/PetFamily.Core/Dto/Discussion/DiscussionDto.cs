namespace PetFamily.Core.Dto.Discussion;

public class DiscussionDto
{
    public Guid Id { get; init; }

    public Guid RelationId { get; init; }

    public List<Guid> UserIds { get; init; } = [];

    public List<MessageDto> Messages { get; set; } = [];

    public string Status { get; init; } = null!;
}