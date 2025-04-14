using System.Text.Json.Serialization;
using PetFamily.Core.Dto.Pet;
using PetFamily.Core.Dto.Shared;

namespace PetFamily.Core.Dto.Volunteer;

public class VolunteerDto
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;

    public int Experience { get; set; } = default;
    
    // public List<PetDto> Pets { get; set; } = [];
    
    [JsonIgnore]
    public bool IsDeleted { get; set; }
}

