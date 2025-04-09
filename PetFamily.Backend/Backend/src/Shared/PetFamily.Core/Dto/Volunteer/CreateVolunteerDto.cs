namespace PetFamily.Core.Dto.Volunteer;

public record CreateVolunteerDto(
    FullNameDto FullName,
    string Email,
    string PhoneNumber,
    string Description,
    int Experience);
