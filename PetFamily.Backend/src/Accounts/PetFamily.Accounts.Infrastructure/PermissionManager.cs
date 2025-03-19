using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Domain.DataModels;

namespace PetFamily.Accounts.Infrastructure;

public class PermissionManager
{
    private readonly AccountsDbContext _accountsDbContext;

    public PermissionManager(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task AddRangeIfNotExist(IEnumerable<string> permissionsToAdd)
    {
        foreach (var permissionCode in permissionsToAdd)
        {
            var isPermissionExist = await _accountsDbContext.Permissions
                .AnyAsync(p => p.Code == permissionCode);

            if (isPermissionExist)
                continue;

            await _accountsDbContext.Permissions.AddAsync(new Permission() { Code = permissionCode });
        }

        await _accountsDbContext.SaveChangesAsync();
    }

    public async Task<Permission?> FindByCodeAsync(string code)
    {
        return await _accountsDbContext.Permissions.FirstOrDefaultAsync(p => p.Code == code);
    }
}