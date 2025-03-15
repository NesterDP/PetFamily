using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.Structs;
using PetFamily.Species.Application;
using PetFamily.Species.Infrastructure.DbContexts;
using PetFamily.Species.Infrastructure.Repositories;

namespace PetFamily.Species.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSpeciesInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContexts(configuration)
            .AddRepositories()
            .AddTransactionManagement();
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
    
    private static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        return services;
    }
    
    private static IServiceCollection AddTransactionManagement(
        this IServiceCollection services)
    {
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(UnitOfWorkSelector.Species);
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        return services;
    }
}