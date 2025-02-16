using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public record CreateVolunteerRequest(
    VolunteerDto VolunteerDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto);
