using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;

namespace PetFamily.Accounts.Infrastructure.EntityManagers;

public class ParticipantAccountManager : IParticipantAccountManager
{
    private readonly AccountsDbContext _accountsDbContext;

    public ParticipantAccountManager(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task CreateParticipantAccount(ParticipantAccount participantAccount)
    {
        await _accountsDbContext.ParticipantAccounts.AddAsync(participantAccount);
        await _accountsDbContext.SaveChangesAsync();
    }
}