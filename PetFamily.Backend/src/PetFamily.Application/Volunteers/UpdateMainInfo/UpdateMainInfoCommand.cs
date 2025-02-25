using PetFamily.Application.Dto.Volunteer;
using PetFamily.Domain.PetContext.ValueObjects.VolunteerVO;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public record UpdateMainInfoCommand(Guid Id,
    FullNameDto FullNameDto,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber);
    