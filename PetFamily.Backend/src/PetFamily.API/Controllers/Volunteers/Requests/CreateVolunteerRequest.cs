using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Application.Volunteers.Commands.Create;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record CreateVolunteerRequest(
    CreateVolunteerDto CreateVolunteerDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto)
{
    public CreateVolunteerCommand ToCommand() => new(CreateVolunteerDto, SocialNetworksDto, TransferDetailsDto);
}