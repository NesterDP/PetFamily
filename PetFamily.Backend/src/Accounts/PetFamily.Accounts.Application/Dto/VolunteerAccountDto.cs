using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Application.Dto;

public class VolunteerAccountDto
{
    public int Experience { get; set; } = 0;

    public List<TransferDetailDto> TransferDetails { get; set; } = [];

    public List<CertificateDto> Certificates { get; set; } = [];
}