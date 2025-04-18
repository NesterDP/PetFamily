using MassTransit;
using PetFamily.Discussions.Infrastructure.Consumers;
using PetFamily.Discussions.Infrastructure.Consumers.Definitions;
using PetFamily.Discussions.Infrastructure.Consumers.Faults;
using PetFamily.Species.Infrastructure.Consumers;
using PetFamily.Volunteers.Infrastructure.Consumers;
using ApprovedRequestConsumerAccounts = PetFamily.Accounts.Infrastructure.Consumers.ApprovedRequestConsumer;
using ApprovedRequestConsumerAccountsDefinition =
    PetFamily.Accounts.Infrastructure.Consumers.Definitions.ApprovedRequestConsumerDefinition;

namespace PetFamily.Web.ApplicationConfiguration;

public static class MessageBusConfigurator
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.AddConsumer<RejectedRequestConsumer, RejectedRequestConsumerDefinition>();
            configure.AddConsumer<ApprovedRequestConsumer, ApprovedRequestConsumerDefinition>();
            configure.AddConsumer<ApprovedRequestConsumerAccounts, ApprovedRequestConsumerAccountsDefinition>();
            configure.AddConsumer<OnReviewRequestConsumer, OnReviewRequestConsumerDefinition>();
            configure.AddConsumer<BreedToPetExistenceEventConsumer>();
            configure.AddConsumer<SpeciesToPetExistenceEventConsumer>();
            configure.AddConsumer<BreedToSpeciesExistenceEventConsumer>();

            configure.AddConsumer<RejectedRequestFaultConsumer>();

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