using FileService.Security.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace FileService.ApplicationConfiguration;

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