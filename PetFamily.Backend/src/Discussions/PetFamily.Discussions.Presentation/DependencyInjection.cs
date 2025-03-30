using Microsoft.Extensions.DependencyInjection;
using PetFamily.Discussions.Contracts;
using PetFamily.Discussions.Presentation.Contracts;

namespace PetFamily.Discussions.Presentation;


public static class DependencyInjection
{
    public static IServiceCollection AddDiscussionsContracts(this IServiceCollection services)
    {
        services.AddScoped<ICreateDiscussionContract, CreateDiscussionContract>();
        services.AddScoped<ICloseDiscussionContract, CloseDiscussionContract>();
        return services;
    }
}