using Microsoft.AspNetCore.Authorization;
using NotificationService.Security.Authorization;

namespace NotificationService.ApplicationConfiguration;

public static class AuthorizationConfigurator
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, InterserviceRequirementHandler>();

        services.AddAuthorization();

        return services;
    }
}