using Microsoft.AspNetCore.Authentication.JwtBearer;
using PetFamily.Core.Options;
using PetFamily.Framework.Security.Authentication;

namespace PetFamily.Web.ApplicationConfiguration;

public static class AuthenticationConfigurator
{
    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(AuthOptions.AUTH).Get<AuthOptions>()
                         ?? throw new ApplicationException("Missing JWT configuration");

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
                    options.TokenValidationParameters = TokenValidationParametersFactory.CreateWithLifeTime(jwtOptions);
                });
        return services;
    }
}