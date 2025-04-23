using MassTransit;
using NotificationService.Consumers;
using NotificationService.Consumers.Definitions;

namespace NotificationService.ApplicationConfiguration;

public static class MessageBusConfigurator
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.AddConsumer<UserWasRegisteredEventConsumer, UserWasRegisteredEventConsumerDefinition>();
            configure.AddConsumer<RequestRequiredRevisionConsumer, RequestRequiredRevisionConsumerDefinition>();
            configure.AddConsumer<RequestWasAmendedConsumer, RequestWasAmendedConsumerDefinition>();
            configure.AddConsumer<RequestWasApprovedConsumer, RequestWasApprovedConsumerDefinition>();
            configure.AddConsumer<RequestWasRejectedConsumer, RequestWasRejectedConsumerDefinition>();
            configure.AddConsumer<RequestWasTakenOnReviewConsumer, RequestWasTakenOnReviewConsumerDefinition>();
            
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