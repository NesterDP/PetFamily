using MassTransit;
using PetFamily.VolunteerRequests.Contracts.Messaging;

namespace PetFamily.Discussions.Infrastructure.Consumers.Faults;

public class RejectedRequestFaultConsumer : IConsumer<Fault<VolunteerRequestWasRejectedEvent>>
{
    public Task Consume(ConsumeContext<Fault<VolunteerRequestWasRejectedEvent>> context)
    {
        // var userId = context.Message.Message.UserId;
        return Task.CompletedTask;
    }
}