using Hangfire;
using Hangfire.PostgreSql;

namespace FileService.ApplicationConfiguration;

public static class HangfireConfigurator
{
    public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(
            config =>
                config.UsePostgreSqlStorage(
                    c =>
                        c.UseNpgsqlConnection(configuration.GetConnectionString("HangfireConnection"))));

        services.AddHangfireServer();
        return services;
    }
}