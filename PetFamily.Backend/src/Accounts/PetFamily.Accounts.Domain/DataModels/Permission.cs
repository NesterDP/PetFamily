namespace PetFamily.Accounts.Domain.DataModels;

public class Permission
{
    public Guid id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    
    public List<RolePermission> RolePermissions { get; set; }
}