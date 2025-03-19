using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;

namespace PetFamily.Accounts.Domain.DataModels;

public class VolunteerAccount
{
    public Guid Id { get; set; }
    
    public int Experience { get; set; }
    
    public List<TransferDetailDto> TransferDetails { get; set; }
    
    public List<CertificateDto> Certificates { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; } // navigation
}