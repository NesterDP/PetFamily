namespace PetFamily.Accounts.Contracts.Dto;

public class UserInfoDto
{
    public FullNameDto FullName { get; init; } = null!;

    public AvatarDto Avatar { get; init; } = null!;

    public string Email { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public List<RoleDto> Roles { get; init; } = [];

    public List<SocialNetworkDto> SocialNetworks { get; init; } = [];

    public ParticipantAccountDto? ParticipantAccount { get; init; }

    public VolunteerAccountDto? VolunteerAccount { get; init; }

    public AdminAccountDto? AdminAccount { get; init; }
}