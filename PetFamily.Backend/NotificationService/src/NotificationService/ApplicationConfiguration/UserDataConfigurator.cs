using NotificationService.Security.Authorization;

namespace NotificationService.ApplicationConfiguration;

public static class UserDataConfigurator
{
    public static IServiceCollection ConfigureUserData(this IServiceCollection services)
    {
        services.AddScoped<UserScopedData>(
            sp =>
            {
                var contextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var httpContext = contextAccessor.HttpContext;
                var userData = new UserScopedData();

                if (httpContext?.User.Identity?.IsAuthenticated ?? false)
                    userData.LoadFromClaims(httpContext.User.Claims);

                return userData;
            });

        return services;
    }
}