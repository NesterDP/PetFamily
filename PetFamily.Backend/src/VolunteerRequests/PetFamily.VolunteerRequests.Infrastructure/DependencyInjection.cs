using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;
using PetFamily.VolunteerRequests.Application.Abstractions;
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
            .AddRepositories();
        return services;
    }
    
    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<WriteDbContext>(_ =>
            new WriteDbContext(configuration.GetConnectionString(InfrastructureConstants.DATABASE)!));
        
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
        return services;
    }
}