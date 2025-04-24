using System.Text.Json.Serialization;

namespace PetFamily.Core.Dto.Volunteer;

public class VolunteerDto
{
    public Guid Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Surname { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public int Experience { get; init; }

    // public List<PetDto> Pets { get; set; } = [];
    [JsonIgnore]
    public bool IsDeleted { get; init; }
}
