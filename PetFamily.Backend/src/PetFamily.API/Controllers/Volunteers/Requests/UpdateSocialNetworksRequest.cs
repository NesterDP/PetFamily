using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.Commands.UpdateSocialNetworks;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateSocialNetworksRequest(IEnumerable<SocialNetworkDto> SocialNetworksDto)
{
    public UpdateSocialNetworksCommand ToCommand(Guid id) => new(id, SocialNetworksDto);
}