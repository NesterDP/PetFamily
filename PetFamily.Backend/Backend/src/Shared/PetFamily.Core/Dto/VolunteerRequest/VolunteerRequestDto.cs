namespace PetFamily.Core.Dto.VolunteerRequest;

public class VolunteerRequestDto
{
    public Guid Id { get; init; }

    public Guid? AdminId { get; init; }

    public Guid UserId { get; init; }

    public string VolunteerInfo { get; init; } = null!;

    public string Status { get; init; } = null!;

    public DateTime CreatedAt { get; init; }

    public string? RevisionComment { get; init; }

    public DateTime? RejectedAt { get; init; }
}