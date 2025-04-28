namespace PetFamily.Core.Dto.Volunteer;

public record CreateVolunteerDto(
    Guid UserId,
    FullNameDto FullName,
    string Email,
    string PhoneNumber,
    int Experience);