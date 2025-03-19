using Microsoft.AspNetCore.Identity;

namespace PetFamily.Accounts.Domain.DataModels;

public class Role : IdentityRole<Guid>
{
    public List<RolePermission> RolePermissions { get; set; } // navigation
}