using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.Commands.Create;

public record CreateVolunteerCommand(
    VolunteerCommandDto VolunteerCommandDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto);