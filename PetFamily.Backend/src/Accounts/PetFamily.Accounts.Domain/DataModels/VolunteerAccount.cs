using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Domain.DataModels;

public class VolunteerAccount
{
    public const string VOLUNTEER = "Volunteer";
    
    public Guid Id { get; set; }
    
    public int Experience { get; set; }
    
    public List<TransferDetail> TransferDetails { get; set; }
    
    public List<CertificateDto> Certificates { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; } // navigation
}