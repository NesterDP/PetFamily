using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasApprovedEventHandlers;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;
using PetFamily.VolunteerRequests.Infrastructure.Outbox;
using PetFamily.VolunteerRequests.Infrastructure.Repositories;
using PetFamily.VolunteerRequests.Infrastructure.TransactionServices;
using Quartz;

namespace PetFamily.VolunteerRequests.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVolunteerRequestsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContexts(configuration)
            .AddTransactionManagement()
            .AddRepositories()
            .AddMediatrService()
            .AddOutbox();

        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));

        services.AddScoped<IReadDbContext, ReadDbContext>(_ =>
            new ReadDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));

        return services;
    }

    private static IServiceCollection AddTransactionManagement(
        this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(UnitOfWorkSelector.VolunteerRequests);
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }

    private static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IVolunteerRequestsRepository, VolunteerRequestsRepository>();
        services.AddScoped<ITestEntitiesRepository, TestEntitiesRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        return services;
    }

    private static IServiceCollection AddMediatrService(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(TestEntityCreationWithTrueStatus).Assembly);
        });

        return services;
    }

    private static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        services.AddScoped<ProcessOutboxMessagesService>();

        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey).WithSimpleSchedule(
                    schedule => schedule.WithIntervalInSeconds(
                            InfrastructureConstants.OUTBOX_TASK_WORKING_INTERVAL_IN_SECONDS)
                        .RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}