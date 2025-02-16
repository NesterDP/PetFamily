namespace PetFamily.Application.Dto.Volunteer;

public record VolunteerDto(
    string FirstName,
    string LastName,
    string Surname,
    string Email,
    string PhoneNumber,
    string Description,
    int Experience);
