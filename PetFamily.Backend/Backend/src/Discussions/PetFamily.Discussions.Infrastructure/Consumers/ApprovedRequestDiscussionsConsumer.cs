using MassTransit;
using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Contracts;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.VolunteerRequests.Contracts.Messaging;

namespace PetFamily.Discussions.Infrastructure.Consumers;

public class ApprovedRequestDiscussionsConsumer : IConsumer<VolunteerRequestWasApprovedEvent>
{
    private readonly ILogger<ApprovedRequestDiscussionsConsumer> _logger;
    private readonly ICloseDiscussionContract _discussionContract;

    public ApprovedRequestDiscussionsConsumer(
        ILogger<ApprovedRequestDiscussionsConsumer> logger,
        ICloseDiscussionContract discussionContract)
    {
        _logger = logger;
        _discussionContract = discussionContract;
    }

    public async Task Consume(ConsumeContext<VolunteerRequestWasApprovedEvent> context)
    {
        var request = new CloseDiscussionRequest(context.Message.RequestId, context.Message.UserId);

        var result = await _discussionContract.CloseDiscussion(request, CancellationToken.None);

        if (result.IsSuccess)
            _logger.LogInformation("Successfully closed discussion with ID = {ID}", result.Value);
        else
            _logger.LogError("Failed to close discussion with ID = {ID}", result.Value);
    }
}