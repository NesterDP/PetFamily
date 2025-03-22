using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;

namespace PetFamily.Accounts.Infrastructure.EntityManagers;

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

    public async Task<HashSet<string>> GetUserPermissionCodes(Guid userId)
    {
        var permissions = await _accountsDbContext.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .ToListAsync();

        return permissions.ToHashSet();
    }
}