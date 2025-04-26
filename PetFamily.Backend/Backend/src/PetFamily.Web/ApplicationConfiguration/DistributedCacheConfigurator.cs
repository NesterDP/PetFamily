namespace PetFamily.Web.ApplicationConfiguration;

public static class DistributedCacheConfigurator
{
    public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(
            options =>
            {
                string connection = configuration.GetConnectionString("Redis") ??
                                    throw new ArgumentNullException(nameof(connection));

                options.Configuration = connection;
            });

        return services;
    }
}