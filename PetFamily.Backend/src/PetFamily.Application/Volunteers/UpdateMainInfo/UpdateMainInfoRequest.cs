using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public record UpdateMainInfoRequest(
    Guid Id,
    UpdateMainInfoDto Dto);

public record UpdateMainInfoDto(
    FullNameDto FullName,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber);
    