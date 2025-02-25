using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid Id,
    SocialNetworksDto Dto);