using MassTransit;
using PetFamily.Discussions.Infrastructure.Consumers;
using VolunteerRequestWasApprovedEventAccountsConsumer =
    PetFamily.Accounts.Infrastructure.Consumers.VolunteerRequestWasApprovedEventConsumer;


namespace PetFamily.Web.ApplicationConfiguration;

public static class MessageBusConfigurator
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.AddConsumer<VolunteerRequestWasRejectedEventConsumer>();
            configure.AddConsumer<VolunteerRequestWasApprovedEventConsumer>();
            configure.AddConsumer<VolunteerRequestWasApprovedEventAccountsConsumer>();
            configure.AddConsumer<VolunteerRequestWasTakenOnReviewEventConsumer>();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["RabbitMQ:Host"]!), h =>
                {
                    h.Username(configuration["RabbitMQ:UserName"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });

                cfg.Durable = true;

                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }
}