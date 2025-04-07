using Microsoft.Extensions.DependencyInjection;
using PetFamily.Species.Contracts;
using PetFamily.Species.Presentation.Contracts;

namespace PetFamily.Species.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddSpeciesContracts(this IServiceCollection services)
    {
        services.AddScoped<IBreedToSpeciesExistenceContract, BreedToSpeciesExistenceContract>();
        return services;
    }
}