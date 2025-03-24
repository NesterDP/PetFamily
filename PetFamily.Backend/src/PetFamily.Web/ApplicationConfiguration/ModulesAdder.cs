using PetFamily.Accounts.Application;
using PetFamily.Accounts.Infrastructure;
using PetFamily.Accounts.Presentation;
using PetFamily.Species.Application;
using PetFamily.Species.Infrastructure;
using PetFamily.Species.Presentation;
using PetFamily.Volunteers.Application;
using PetFamily.Volunteers.Infrastructure;
using PetFamily.Volunteers.Presentation;

namespace PetFamily.Web.ApplicationConfiguration;

public static class ModulesAdder
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAccountsInfrastructure(configuration)
            .AddAccountsApplication()
            .AddAccountsContracts();
        
        return services;
    }
    
    public static IServiceCollection AddVolunteersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddVolunteersInfrastructure(configuration)
            .AddVolunteersApplication()
            .AddVolunteersContracts();

        return services;
    }
    
    public static IServiceCollection AddSpeciesModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSpeciesInfrastructure(configuration)
            .AddSpeciesApplication()
            .AddSpeciesContracts();
        
        return services;
    }

    public static void AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        AddAccountsModule(services, configuration);
        AddVolunteersModule(services, configuration);
        AddSpeciesModule(services, configuration);
    }
}