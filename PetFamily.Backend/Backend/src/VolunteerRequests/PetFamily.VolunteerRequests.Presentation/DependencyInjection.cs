using Microsoft.Extensions.DependencyInjection;

namespace PetFamily.VolunteerRequests.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddVolunteerRequestsContracts(this IServiceCollection services)
    {
        return services;
    }
}