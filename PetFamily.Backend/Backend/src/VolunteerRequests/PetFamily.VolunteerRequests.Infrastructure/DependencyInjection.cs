using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;
using PetFamily.VolunteerRequests.Application.Abstractions;
using PetFamily.VolunteerRequests.Application.EventHandlers.VolunteerRequestWasApprovedEventHandlers;
using PetFamily.VolunteerRequests.Infrastructure.DbContexts;
using PetFamily.VolunteerRequests.Infrastructure.Repositories;
using PetFamily.VolunteerRequests.Infrastructure.TransactionServices;

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
            .AddServices();
        
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
        return services;
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(TestEntityCreationWithTrueStatus).Assembly);
        });
        
        return services;
    }
}