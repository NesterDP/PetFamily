using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.API.Controllers.Volunteers.Requests;

public record UpdateMainInfoRequest(
    FullNameDto FullNameDto,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber);
    