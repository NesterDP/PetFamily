using Microsoft.AspNetCore.Identity;
using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Accounts.Domain.DataModels;

public class User : IdentityUser<Guid>
{
    public string Photo { get; set; }
    
    public List<SocialNetworkDto> SocialNetworks = [];
    
    public FullNameDto FullName { get; set; }
    
    //public Guid RoleId { get; set; }
    
    public ParticipantAccount? ParticipantAccount { get; set; } // navigation
    
    public VolunteerAccount? VolunteerAccount { get; set; } // navigation
    
    public AdminProfile? AdminProfile { get; set; } // navigation
}

