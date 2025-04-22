namespace PetFamily.Accounts.Contracts.Dto;

public record CertificateDto(
    string Name,
    string GivenBy,
    int YearOfAcquisition);