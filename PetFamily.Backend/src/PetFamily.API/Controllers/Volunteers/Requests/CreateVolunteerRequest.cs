using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record CreateVolunteerRequest(
    VolunteerDto VolunteerDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto);