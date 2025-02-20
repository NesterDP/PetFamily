using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public record UpdateSocialNetworksRequest(
    Guid Id,
    SocialNetworksDto Dto);