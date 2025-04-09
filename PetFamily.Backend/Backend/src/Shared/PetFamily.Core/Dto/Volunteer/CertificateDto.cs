namespace PetFamily.Core.Dto.Volunteer;

public record CertificateDto(
    string Name,
    string GivenBy,
    int YearOfAcquisition);