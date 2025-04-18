using MassTransit;
using Microsoft.Extensions.Logging;
using PetFamily.Discussions.Contracts;
using PetFamily.Discussions.Contracts.Requests;
using PetFamily.VolunteerRequests.Contracts.Messaging;

namespace PetFamily.Discussions.Infrastructure.Consumers;

public class VolunteerRequestWasTakenOnReviewEventConsumer : IConsumer<VolunteerRequestWasTakenOnReviewEvent>
{
    private readonly ILogger<VolunteerRequestWasTakenOnReviewEventConsumer> _logger;
    private readonly ICreateDiscussionContract _discussionContract;

    public VolunteerRequestWasTakenOnReviewEventConsumer(
        ILogger<VolunteerRequestWasTakenOnReviewEventConsumer> logger,
        ICreateDiscussionContract discussionContract)
    {
        _logger = logger;
        _discussionContract = discussionContract;
    }

    public async Task Consume(ConsumeContext<VolunteerRequestWasTakenOnReviewEvent> context)
    {
        List<Guid> userIds = [context.Message.UserId, context.Message.AdminId];
        var request = new CreateDiscussionRequest(context.Message.RequestId, userIds);

        var result = await _discussionContract.CreateDiscussion(request, CancellationToken.None);
        
        if (result.IsSuccess)
            _logger.LogInformation("Successfully created discussion with ID = {ID}", result.Value);
        else
            _logger.LogError("Failed to create discussion for request with requestID = {ID}", request.RelationId);
    }
}