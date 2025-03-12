using PetFamily.Application.Abstractions;
using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.Commands.Create;

public record CreateVolunteerCommand(
    CreateVolunteerDto CreateVolunteerDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto) : ICommand;