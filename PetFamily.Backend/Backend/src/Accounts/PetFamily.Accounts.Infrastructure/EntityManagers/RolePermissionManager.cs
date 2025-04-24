using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;

namespace PetFamily.Accounts.Infrastructure.EntityManagers;

public class RolePermissionManager
{
    private readonly AccountsDbContext _accountsDbContext;

    public RolePermissionManager(AccountsDbContext accountsDbContext)
    {
        _accountsDbContext = accountsDbContext;
    }

    public async Task AddRangeIfNotExist(Guid roleId, IEnumerable<string> permissions)
    {
        foreach (string? permissionCode in permissions)
        {
            var permission = await _accountsDbContext.Permissions
                .FirstOrDefaultAsync(p => p.Code == permissionCode);

            if (permission == null)
                throw new ApplicationException($"Permission with Code {permissionCode} not found");

            bool rolePermissionExist = await _accountsDbContext.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission!.Id);

            if (rolePermissionExist)
                continue;

            await _accountsDbContext.RolePermissions.AddAsync(
                new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission!.Id,
                });
        }

        await _accountsDbContext.SaveChangesAsync();
    }
}