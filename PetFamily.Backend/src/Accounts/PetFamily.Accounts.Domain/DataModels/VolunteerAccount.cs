using PetFamily.Core.Dto.Shared;
using PetFamily.Core.Dto.Volunteer;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Domain.DataModels;

public class VolunteerAccount
{
    public Guid Id { get; set; }

    public int Experience { get; set; } = 0;

    public List<TransferDetail> TransferDetails { get; set; } = [];

    public List<CertificateDto> Certificates { get; set; } = [];
    
    public Guid UserId { get; set; } // navigation
    
    public User User { get; set; } // navigation
    
    public VolunteerAccount() { } // ef core

    public VolunteerAccount(User user)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        User = user;
    }
}