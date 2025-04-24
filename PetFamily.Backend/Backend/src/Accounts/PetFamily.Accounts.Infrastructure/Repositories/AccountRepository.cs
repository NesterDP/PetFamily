using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.SharedKernel.CustomErrors;

namespace PetFamily.Accounts.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsDbContext _dbContext;

    public AccountRepository(AccountsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User, Error>> GetUserById(Guid id)
    {
        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .Include(u => u.ParticipantAccount)
            .Include(u => u.VolunteerAccount)
            .Include(u => u.AdminAccount)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return Errors.General.ValueNotFound(id);

        return user;
    }
}