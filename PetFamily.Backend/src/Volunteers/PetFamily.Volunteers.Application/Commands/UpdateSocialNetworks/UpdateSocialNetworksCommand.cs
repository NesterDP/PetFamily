using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Application.Commands.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid Id,
    IEnumerable<SocialNetworkDto> SocialNetworks) : ICommand;