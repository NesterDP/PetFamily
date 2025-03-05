using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.Commands.UpdateSocialNetworks;

public record UpdateSocialNetworksCommand(
    Guid Id,
    SocialNetworksDto Dto) : ICommand;