using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Accounts.Infrastructure.EntityManagers;
using PetFamily.Accounts.Infrastructure.Providers;
using PetFamily.Accounts.Infrastructure.Seeding;
using PetFamily.Accounts.Infrastructure.TransactionServices;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Options;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;

namespace PetFamily.Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ITokenProvider, JwtTokenProvider>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JWT));
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.ADMIN));

        services.RegisterIdentity();

        services.AddDbContext(configuration);

        services.AddSingleton<AccountsSeeder>();
        services.AddScoped<AccountsSeederService>();

        services.AddTransactionManagement();

        return services;
    }

    public static void RegisterIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<User, Role>(options => { options.User.RequireUniqueEmail = true; })
            .AddEntityFrameworkStores<AccountsDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<PermissionManager>();
        services.AddScoped<RolePermissionManager>();
        services.AddScoped<AdminAccountManager>();
        services.AddScoped<IParticipantAccountManager, ParticipantAccountManager>();
    }

    private static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AccountsDbContext>(_ =>
            new AccountsDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        return services;
    }
    
    private static IServiceCollection AddTransactionManagement(
        this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(UnitOfWorkSelector.Accounts);
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        return services;
    }
}