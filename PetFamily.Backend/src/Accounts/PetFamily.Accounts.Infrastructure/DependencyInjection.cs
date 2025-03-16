using System.Text;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PetFamily.Accounts.Application;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ITokenProvider, JwtTokenProvider>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JWT));
        
        services.RegisterIdentity();

        services
            .AddDbContext(configuration)
            .ConfigureAuthentication(configuration)
            .AddAuthorization();

        return services;
    }

    public static void RegisterIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<User, Role>(options => { options.User.RequireUniqueEmail = true; })
            .AddEntityFrameworkStores<AuthorizationDbContext>()
            .AddDefaultTokenProviders();
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuthorizationDbContext>(_ =>
            new AuthorizationDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        return services;
    }

    private static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services
            //.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
            {
                var jwtOption = configuration.GetSection(JwtOptions.JWT).Get<JwtOptions>()
                                ?? throw new ApplicationException("Missing JWT configuration");
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOption.Issuer,
                    ValidAudience = jwtOption.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOption.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

        return services;
    }
}