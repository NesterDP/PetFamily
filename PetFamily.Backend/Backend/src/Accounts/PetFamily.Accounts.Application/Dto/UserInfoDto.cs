using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Application.Dto;

public class UserInfoDto
{
    public FullNameDto FullName { get; set; } 

    public AvatarDto Avatar { get; set; }
    
    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;
    
    public List<RoleDto> Roles { get; set; } = [];

    public List<SocialNetworkDto> SocialNetworks { get; set; } = [];

    public ParticipantAccountDto? ParticipantAccount { get; set; }
    
    public VolunteerAccountDto? VolunteerAccount { get; set; }
    
    public AdminAccountDto? AdminAccount { get; set; }
}