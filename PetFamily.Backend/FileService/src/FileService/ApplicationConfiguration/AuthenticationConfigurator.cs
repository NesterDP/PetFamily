using FileService.Security;
using FileService.Security.Authentication;
using FileService.Security.Authorization;
using FileService.Security.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FileService.ApplicationConfiguration;

public static class AuthenticationConfigurator
{
    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.AUTH));

        var authOptions = configuration.GetSection(AuthOptions.AUTH).Get<AuthOptions>()
                          ?? throw new ApplicationException("Missing AUTH configuration");

        services
            .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters =
                        TokenValidationParametersFactory.CreateWithLifeTime(authOptions);
                })
            .AddInterserviceKey(authOptions.ServicesKey);

        return services;
    }

    private static void AddInterserviceKey(this AuthenticationBuilder builder, string key)
    {
        builder.AddScheme<InterserviceAuthenticationOptions, InterserviceAuthenticationHandler>(
            InterserviceKeyDefaults.AuthenticationScheme,
            options => options.ExpectedKey = key);
    }
}