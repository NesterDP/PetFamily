using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Abstractions;
using PetFamily.Accounts.Domain.DataModels;
using PetFamily.Accounts.Infrastructure.DbContexts;
using PetFamily.Accounts.Infrastructure.EntityManagers;
using PetFamily.Accounts.Infrastructure.Outbox;
using PetFamily.Accounts.Infrastructure.Providers;
using PetFamily.Accounts.Infrastructure.Repositories;
using PetFamily.Accounts.Infrastructure.Seeding;
using PetFamily.Accounts.Infrastructure.TransactionServices;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Options;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;
using Quartz;

namespace PetFamily.Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddProviders()
            .AddConfigurations(configuration)
            .RegisterIdentity()
            .AddDbContexts(configuration)
            .AddSeeding()
            .AddTransactionManagement()
            .AddRepositories()
            .AddOutbox();

        return services;
    }

    private static IServiceCollection RegisterIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<User, Role>(options => { options.User.RequireUniqueEmail = true; })
            .AddEntityFrameworkStores<AccountsDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<PermissionManager>();
        services.AddScoped<RolePermissionManager>();
        services.AddScoped<IAccountManager, AccountManager>();
        services.AddScoped<IRefreshSessionManager, RefreshSessionManager>();

        return services;
    }

    private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AccountsDbContext>(
            _ => new AccountsDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        return services;
    }

    private static IServiceCollection AddTransactionManagement(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(UnitOfWorkSelector.Accounts);
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        return services;
    }

    private static IServiceCollection AddSeeding(this IServiceCollection services)
    {
        services.AddSingleton<AccountsSeeder>();
        services.AddScoped<AccountsSeederService>();
        return services;
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JWT));
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.ADMIN));
        services.Configure<RefreshSessionOptions>(configuration.GetSection(RefreshSessionOptions.REFRESH_SESSION));

        return services;
    }

    private static IServiceCollection AddProviders(this IServiceCollection services)
    {
        services.AddTransient<ITokenProvider, JwtTokenProvider>();
        return services;
    }

    private static void AddOutbox(this IServiceCollection services)
    {
        services.AddScoped<ProcessOutboxMessagesService>();

        services.AddQuartz(
            configure =>
        {
            var jobKey = new JobKey(Guid.NewGuid().ToString());

            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(
                    trigger => trigger.ForJob(jobKey).WithSimpleSchedule(
                    schedule => schedule.WithIntervalInSeconds(
                            InfrastructureConstants.OUTBOX_TASK_WORKING_INTERVAL_IN_SECONDS)
                        .RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}