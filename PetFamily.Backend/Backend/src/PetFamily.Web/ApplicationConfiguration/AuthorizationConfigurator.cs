using Microsoft.AspNetCore.Authorization;
using PetFamily.Framework.Authorization;

namespace PetFamily.Web.ApplicationConfiguration;

public static class AuthorizationConfigurator
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();

        services.AddAuthorization();

        return services;
    }
}