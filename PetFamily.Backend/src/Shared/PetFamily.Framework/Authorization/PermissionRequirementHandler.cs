using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace PetFamily.Framework.Authorization;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    private readonly IServiceScopeFactory _scopeFactory;
    public PermissionRequirementHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

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