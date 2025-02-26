using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.Create;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record CreateVolunteerRequest(
    VolunteerDto VolunteerDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto)
{
    public CreateVolunteerCommand ToCommand() => new(VolunteerDto, SocialNetworksDto, TransferDetailsDto);
}