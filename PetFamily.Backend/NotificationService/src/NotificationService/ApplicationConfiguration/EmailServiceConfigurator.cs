using NotificationService.Core.Options;
using NotificationService.Services;

namespace NotificationService.ApplicationConfiguration;

public static class EmailServiceConfigurator
{
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SMTP_OPTIONS));
        services.AddTransient<EmailService>();

        return services;
    }
}