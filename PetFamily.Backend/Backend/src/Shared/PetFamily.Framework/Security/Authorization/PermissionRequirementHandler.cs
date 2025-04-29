using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Contracts;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.Core;

namespace PetFamily.Framework.Security.Authorization;

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
        using var scope = _scopeFactory.CreateScope();
        var contract = scope.ServiceProvider.GetRequiredService<IGetUserPermissionCodesContract>();

        string? userIdString = context.User.Claims
            .FirstOrDefault(claim => claim.Type == CustomClaims.ID)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
        {
            context.Fail();
            return;
        }

        var request = new GetUserPermissionCodesRequest(userId);
        var permissionCodes = await contract.GetUserPermissionCodes(request);

        if (!permissionCodes.Contains(permission.Code))
        {
            context.Fail();
            return;
        }

        context.Succeed(permission);
    }
}