using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.Volunteers.Application.VolunteersManagement.Commands.Create;

namespace PetFamily.Volunteers.Presentation.Volunteers.Requests;

public record CreateVolunteerRequest(
    CreateVolunteerDto CreateVolunteerDto,
    IEnumerable<SocialNetworkDto> SocialNetworksDto,
    IEnumerable<TransferDetailDto> TransferDetailsDto)
{
    public CreateVolunteerCommand ToCommand() => new(CreateVolunteerDto, SocialNetworksDto, TransferDetailsDto);
}