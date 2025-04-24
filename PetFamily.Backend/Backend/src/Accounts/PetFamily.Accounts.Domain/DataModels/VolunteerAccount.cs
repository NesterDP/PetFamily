using PetFamily.Accounts.Contracts.Dto;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.Accounts.Domain.DataModels;

public class VolunteerAccount
{
    public Guid Id { get; init; }

    public int Experience { get; init; }

    public List<TransferDetail> TransferDetails { get; init; } = [];

    public List<CertificateDto> Certificates { get; init; } = [];

    public Guid UserId { get; init; } // navigation

    public User User { get; init; } = null!; // navigation

    public VolunteerAccount(User user)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        User = user;
    }

    private VolunteerAccount() { } // ef core
}