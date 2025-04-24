namespace PetFamily.Accounts.Contracts.Dto;

public class VolunteerAccountDto
{
    public int Experience { get; set; }

    public List<TransferDetailDto> TransferDetails { get; set; } = [];

    public List<CertificateDto> Certificates { get; set; } = [];
}