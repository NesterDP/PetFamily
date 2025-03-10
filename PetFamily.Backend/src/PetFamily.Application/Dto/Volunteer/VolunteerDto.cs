using System.Text.Json.Serialization;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Dto.Volunteer;

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
    public SocialNetworkDto[] SocialNetworks { get; set; } = [];
    
    public TransferDetailDto[] TransferDetails { get; set; } = [];
    
    public PetDto[] Pets { get; set; } = [];
    
    [JsonIgnore]
    public bool IsDeleted { get; set; }
}

