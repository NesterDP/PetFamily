namespace PetFamily.Accounts.Infrastructure.EntityManagers.ManagersOptions;

public class RolePermissionOptions
{
    public Dictionary<string, string[]> Permissions { get; init; } = [];

    public Dictionary<string, string[]> Roles { get; init; } = [];
}