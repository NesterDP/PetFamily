namespace PetFamily.Core.Dto.Discussion;

public class DiscussionDto
{
    public Guid Id { get; set; }
    public Guid RelationId { get; set; }
    public List<Guid> UserIds { get; set; } = [];
    public List<MessageDto> Messages { get; set; } = [];
    public string Status { get; set; }
}

