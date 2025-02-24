using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateSocialNetworksRequest(
    Guid Id,
    SocialNetworksDto SocialNetworksDto);