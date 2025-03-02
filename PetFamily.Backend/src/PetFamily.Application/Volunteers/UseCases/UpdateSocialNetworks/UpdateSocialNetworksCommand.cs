using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.UseCases.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid Id,
    SocialNetworksDto Dto);