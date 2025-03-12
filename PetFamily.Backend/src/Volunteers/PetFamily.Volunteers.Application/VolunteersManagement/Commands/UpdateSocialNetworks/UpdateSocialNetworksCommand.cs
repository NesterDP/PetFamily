using PetFamily.Core.Abstractions;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Volunteers.Application.VolunteersManagement.Commands.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid Id,
    IEnumerable<SocialNetworkDto> SocialNetworks) : ICommand;