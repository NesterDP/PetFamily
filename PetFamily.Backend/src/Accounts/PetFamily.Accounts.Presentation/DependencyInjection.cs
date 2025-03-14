using Microsoft.Extensions.DependencyInjection;

namespace PetFamily.Accounts.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsContracts(this IServiceCollection services)
    {
        return services;
    }
}