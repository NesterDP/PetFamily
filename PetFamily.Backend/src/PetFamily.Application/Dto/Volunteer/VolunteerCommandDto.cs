namespace PetFamily.Application.Dto.Volunteer;

public record VolunteerCommandDto(
    FullNameDto FullName,
    string Email,
    string PhoneNumber,
    string Description,
    int Experience);
