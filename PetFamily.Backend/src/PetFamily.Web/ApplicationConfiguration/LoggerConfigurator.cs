using Serilog;
using Serilog.Events;

namespace PetFamily.Web.ApplicationConfiguration;

public static class LoggerConfigurator
{
    public static void Configure(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Seq(configuration.GetConnectionString("Seq")
                         ?? throw new ArgumentNullException("Seq"))
            .WriteTo.Console()
            .Enrich.WithMachineName()
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();
    }
}