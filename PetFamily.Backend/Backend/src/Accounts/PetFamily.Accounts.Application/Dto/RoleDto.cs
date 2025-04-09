namespace PetFamily.Accounts.Application.Dto;

public class RoleDto
{
    public RoleDto(string roleName) => RoleName = roleName;
    public string RoleName { get; set; } = string.Empty;
}