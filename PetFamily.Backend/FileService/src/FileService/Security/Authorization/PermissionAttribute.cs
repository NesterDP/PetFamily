using Microsoft.AspNetCore.Authorization;

namespace FileService.Security.Authorization;

public class PermissionAttribute : AuthorizeAttribute, IAuthorizationRequirement
{
    public string Code { get; set; }

    public PermissionAttribute(string code)
        : base(code)
    {
        Code = code;
    }
}