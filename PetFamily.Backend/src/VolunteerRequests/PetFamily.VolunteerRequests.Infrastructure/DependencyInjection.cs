using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PetFamily.VolunteerRequests.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddVolunteerRequestsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}