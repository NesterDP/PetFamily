using Microsoft.Extensions.Http;
using PetFamily.Framework.Security.HttpHandlers;

namespace PetFamily.Web.ApplicationConfiguration;

public static class HttpClientsConfigurator
{
    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<HttpTrackerHandler>();

        services.ConfigureAll<HttpClientFactoryOptions>(
            options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(
                    builder =>
                    {
                        builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<HttpTrackerHandler>());
                    });
            });

        return services;
    }
}