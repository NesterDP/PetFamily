namespace PetFamily.Core.Dto.VolunteerRequest;

public class VolunteerRequestDto
{
    public Guid Id { get; set; }
    public Guid? AdminId { get; set; }
    public Guid UserId { get; set; }
    public string VolunteerInfo { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? RevisionComment { get; set; }
    public DateTime? RejectedAt { get; set; }
}