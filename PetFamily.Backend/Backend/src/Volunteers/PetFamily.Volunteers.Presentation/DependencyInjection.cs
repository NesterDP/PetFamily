using Microsoft.Extensions.DependencyInjection;
using PetFamily.Volunteers.Contracts;
using PetFamily.Volunteers.Presentation.Contracts;

namespace PetFamily.Volunteers.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddVolunteersContracts(this IServiceCollection services)
    {
        services.AddScoped<IBreedToPetExistenceContract, BreedToPetExistenceContract>();
        services.AddScoped<ISpeciesToPetExistenceContract, SpeciesToPetExistenceContract>();
        return services;
    }
}