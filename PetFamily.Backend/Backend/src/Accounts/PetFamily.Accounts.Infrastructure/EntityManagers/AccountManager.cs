using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;

namespace PetFamily.Accounts.Infrastructure.EntityManagers;

public class AccountManager : IAccountManager
{
    private readonly AccountsDbContext _accountsDbContext;

    public AccountManager(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task CreateParticipantAccount(ParticipantAccount participantAccount)
    {
        await _accountsDbContext.ParticipantAccounts.AddAsync(participantAccount);
        await _accountsDbContext.SaveChangesAsync();
    }

    public async Task CreateVolunteerAccount(VolunteerAccount volunteerAccount)
    {
        await _accountsDbContext.VolunteerAccounts.AddAsync(volunteerAccount);
        await _accountsDbContext.SaveChangesAsync();
    }

    public async Task CreateAdminAccount(AdminAccount adminAccount)
    {
        await _accountsDbContext.AdminAccounts.AddAsync(adminAccount);
        await _accountsDbContext.SaveChangesAsync();
    }
}