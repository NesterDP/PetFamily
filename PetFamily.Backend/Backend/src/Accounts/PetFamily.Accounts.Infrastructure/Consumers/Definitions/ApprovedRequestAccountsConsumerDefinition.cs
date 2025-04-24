using MassTransit;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.Accounts.Infrastructure.Consumers.Definitions;

public class ApprovedRequestAccountsConsumerDefinition : ConsumerDefinition<ApprovedRequestAccountsConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ApprovedRequestAccountsConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);

        endpointConfigurator.UseMessageRetry(r => r.Immediate(InfrastructureConstants.DEFAULT_RETRY_ATTEMPTS));

        endpointConfigurator.UseDelayedRedelivery(
            r => r.Intervals(
                TimeSpan.FromMinutes(InfrastructureConstants.DEFAULT_FIRST_RETRY_ATTEMPT_TIME),
                TimeSpan.FromMinutes(InfrastructureConstants.DEFAULT_SECOND_RETRY_ATTEMPT_TIME),
                TimeSpan.FromMinutes(InfrastructureConstants.DEFAULT_THIRD_RETRY_ATTEMPT_TIME)));
    }
}