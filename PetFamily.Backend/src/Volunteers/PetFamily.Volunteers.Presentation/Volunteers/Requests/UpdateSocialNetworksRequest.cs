using PetFamily.Core.Dto.Volunteer;
using PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdateSocialNetworks;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record UpdateSocialNetworksRequest(IEnumerable<SocialNetworkDto> SocialNetworksDto)
{
    public UpdateSocialNetworksCommand ToCommand(Guid id) => new(id, SocialNetworksDto);
}