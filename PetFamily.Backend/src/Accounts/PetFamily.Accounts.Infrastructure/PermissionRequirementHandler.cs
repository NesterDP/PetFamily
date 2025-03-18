using Microsoft.AspNetCore.Authorization;

namespace PetFamily.Accounts.Infrastructure;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAttribute permission)
    {
        var userPermission = context.User.Claims.FirstOrDefault(c => c.Type == permission.Code);
        if (userPermission is null)
            return;

        if (userPermission.Value != permission.Code)
            return;

        context.Succeed(permission);
    }
}