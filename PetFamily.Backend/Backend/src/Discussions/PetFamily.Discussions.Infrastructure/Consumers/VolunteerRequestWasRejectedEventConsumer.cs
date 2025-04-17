using MassTransit;
using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Contracts;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.VolunteerRequests.Contracts.Messaging;

namespace PetFamily.Discussions.Infrastructure.Consumers;

public class VolunteerRequestWasRejectedEventConsumer : IConsumer<VolunteerRequestWasRejectedEvent>
{
    private readonly ILogger<VolunteerRequestWasRejectedEventConsumer> _logger;
    private readonly ICloseDiscussionContract _discussionContract;

    public VolunteerRequestWasRejectedEventConsumer(
        ILogger<VolunteerRequestWasRejectedEventConsumer> logger,
        ICloseDiscussionContract discussionContract)
    {
        _logger = logger;
        _discussionContract = discussionContract;
    }

    public async Task Consume(ConsumeContext<VolunteerRequestWasRejectedEvent> context)
    {
        var request = new CloseDiscussionRequest(context.Message.RequestId, context.Message.UserId);

        var result = await _discussionContract.CloseDiscussion(request, CancellationToken.None);
        
        if (result.IsSuccess)
            _logger.LogInformation("Successfully closed discussion for event = \"VolunteerRequestWasRejected\"");
        else
            _logger.LogError("Failed to close discussion for event = \"VolunteerRequestWasRejected\"");
    }
}