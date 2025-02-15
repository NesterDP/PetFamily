using PetFamily.Application.Dto.Shared;
using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;
using PetFamily.Domain.Shared.SharedVO;

namespace PetFamily.API.Requests;

public record CreateVolunteerRequest(
    VolunteerDto VolunteerDto,
    List<SocialNetworkDto> SocialNetworkDto,
    List<TransferDetailDto> TransferDetailListDto
    );
        
