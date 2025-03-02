using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.UseCases.UpdateSocialNetworks;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateSocialNetworksRequest(SocialNetworksDto SocialNetworksDto)
{
    public UpdateSocialNetworksCommand ToCommand(Guid id) => new(id, SocialNetworksDto);
}