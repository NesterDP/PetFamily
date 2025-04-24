using Microsoft.AspNetCore.Identity;

namespace PetFamily.Accounts.Domain.DataModels;

public class Role : IdentityRole<Guid>
{
    public List<User> Users { get; init; } = null!; // navigation

    public List<RolePermission> RolePermissions { get; init; } = null!; // navigation
}