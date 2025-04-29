using Microsoft.AspNetCore.Authorization;

namespace NotificationService.Security.Authorization;

public class InterserviceRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAttribute permission)
    {
        if (context.User.HasClaim(c => c is { Type: "IsService", Value: "true" }))
        {
            context.Succeed(permission);
        }

        return Task.CompletedTask;
    }
}