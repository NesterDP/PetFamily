using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.Core.Messaging;
using PetFamily.Core.Options;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure.BackgroundServices;
using PetFamily.Volunteers.Infrastructure.DbContexts;
using PetFamily.Volunteers.Infrastructure.MessageQueues;
using PetFamily.Volunteers.Infrastructure.Repositories;
using PetFamily.Volunteers.Infrastructure.Services;
using PetFamily.Volunteers.Infrastructure.TransactionServices;

namespace PetFamily.Volunteers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVolunteersInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContexts(configuration)
            .AddRepositories()
            .AddTransactionManagement()
            .AddHostedServices()
            .AddMessageQueues()
            .AddServices(configuration);
        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<WriteDbContext>(
            _ =>
                new WriteDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));

        services.AddScoped<IReadDbContext, ReadDbContext>(
            _ =>
                new ReadDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IVolunteersRepository, VolunteerRepository>();
        return services;
    }

    private static IServiceCollection AddTransactionManagement(this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(UnitOfWorkSelector.Volunteers);
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }

    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<DeleteExpiredEntitiesBackgroundService>();
        return services;
    }

    private static IServiceCollection AddMessageQueues(this IServiceCollection services)
    {
        services.AddSingleton<IMessageQueue<IEnumerable<FileInfo>>, InMemoryMessageQueue<IEnumerable<FileInfo>>>();
        return services;
    }

    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DeleteExpiredEntitiesService>();

        services.Configure<ExpiredEntitiesDeletionOptions>(
            configuration.GetSection(ExpiredEntitiesDeletionOptions.EXPIRED_ENTITIES_DELETION));
    }
}