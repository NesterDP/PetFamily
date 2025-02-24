using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.Create;

public record CreateVolunteerCommand(
    VolunteerDto VolunteerDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto);