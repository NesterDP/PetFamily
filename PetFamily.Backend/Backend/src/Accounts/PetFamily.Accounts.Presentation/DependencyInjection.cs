using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Contracts;
using PetFamily.Accounts.Presentation.Contracts;

namespace PetFamily.Accounts.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsContracts(this IServiceCollection services)
    {
        services.AddScoped<IGetUserPermissionCodesContract, GetUserPermissionCodesContract>();
        services.AddScoped<ICreateVolunteerAccountContract, CreateVolunteerAccountContract>();
        services.AddScoped<IGetUserInfoByUserIdContract, GetUserInfoByUserIdContract>();
        return services;
    }
}