using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.UseCases.Create;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record CreateVolunteerRequest(
    VolunteerCommandDto VolunteerCommandDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto)
{
    public CreateVolunteerCommand ToCommand() => new(VolunteerCommandDto, SocialNetworksDto, TransferDetailsDto);
}