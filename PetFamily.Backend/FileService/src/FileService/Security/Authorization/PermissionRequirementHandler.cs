using Microsoft.AspNetCore.Authorization;

namespace FileService.Security.Authorization;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    // ReSharper disable once EmptyConstructor
    public PermissionRequirementHandler() { }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAttribute permission)
    {
        /*
        Если есть необходимость получать разрешения по ID с БД - раскомментировать
        Иначе берем их прямо из клеймов

        using var scope = _scopeFactory.CreateScope();
        var contract = scope.ServiceProvider.GetRequiredService<IGetUserPermissionCodesContract>();

        string? userIdString = context.User.Claims
            .FirstOrDefault(claim => claim.Type == CustomClaims.ID)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return;
        }

        var request = new GetUserPermissionCodesRequest(userId);
        var permissionCodes = await contract.GetUserPermissionCodes(request);
        */

        var permissionCodes = context.User.Claims
            .Where(c => c.Type == CustomClaims.PERMISSION)
            .Select(c => c.Value)
            .ToList();

        if (!permissionCodes.Contains(permission.Code))
        {
            return Task.CompletedTask;
        }

        context.Succeed(permission);
        return Task.CompletedTask;
    }
}