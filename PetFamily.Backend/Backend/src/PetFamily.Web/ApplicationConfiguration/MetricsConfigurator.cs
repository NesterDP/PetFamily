using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace PetFamily.Web.ApplicationConfiguration;

public static class MetricsConfigurator
{
    public static IServiceCollection AddAppMetric(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(
                b => b
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PetFamily.Web"))
                    .AddMeter("PetFamily")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddPrometheusExporter());

        return services;
    }
}