namespace PetFamily.Accounts.Domain.DataModels;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Role Role { get; set; } // navigation

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } // navigation
}