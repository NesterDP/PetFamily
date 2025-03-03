using System.Text.Json.Serialization;
using PetFamily.Application.Dto.Pet;
using PetFamily.Application.Dto.Shared;

namespace PetFamily.Application.Dto.Volunteer;

public class VolunteerDto
{
    
    public Guid Id { get; init; }
    
    public string FirstName { get; init; } = string.Empty;
    
    public string LastName { get; init; } = string.Empty;
    
    public string Surname { get; init; } = string.Empty;
    
    public string PhoneNumber { get; init; } = string.Empty;
    
    public string Email { get; init; } = string.Empty;
    
    public string Description { get; init; } = string.Empty;

    public int Experience { get; init; } = default;
    public SocialNetworkDto[] SocialNetworks { get; init; } = [];
    
    public TransferDetailDto[] TransferDetails { get; init; } = [];
    
    public PetDto[] Pets { get; init; } = [];
}

