namespace PetFamily.Application.Dto.Volunteer;

public record VolunteerDto(
    FullNameDto FullName,
    string Email,
    string PhoneNumber,
    string Description,
    int Experience);
