using Microsoft.AspNetCore.Identity;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Accounts.Domain.DataModels;

public class User : IdentityUser<Guid>
{
    public List<SocialNetworkDto> SocialNetworks = [];
}

