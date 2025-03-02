using PetFamily.Application.Dto.Volunteer;

namespace PetFamily.Application.Volunteers.UseCases.UpdateMainInfo;

public record UpdateMainInfoCommand(Guid Id,
    FullNameDto FullNameDto,
    string Email,
    string Description,
    int Experience,
    string PhoneNumber);
    