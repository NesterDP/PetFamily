namespace PetFamily.Accounts.Domain.DataModels;

public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    
    public List<RolePermission> RolePermissions { get; set; }
}