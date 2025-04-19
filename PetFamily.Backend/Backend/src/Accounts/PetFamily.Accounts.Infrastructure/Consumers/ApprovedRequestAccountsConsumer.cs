using MassTransit;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Contracts;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.VolunteerRequests.Contracts.Messaging;

namespace PetFamily.Accounts.Infrastructure.Consumers;

public class ApprovedRequestAccountsConsumer : IConsumer<VolunteerRequestWasApprovedEvent>
{
    private readonly ILogger<ApprovedRequestAccountsConsumer> _logger;
    private readonly ICreateVolunteerAccountContract _accountContract;

    public ApprovedRequestAccountsConsumer(
        ILogger<ApprovedRequestAccountsConsumer> logger,
        ICreateVolunteerAccountContract accountContract)
    {
        _logger = logger;
        _accountContract = accountContract;
    }

    public async Task Consume(ConsumeContext<VolunteerRequestWasApprovedEvent> context)
    {
        var request = new CreateVolunteerAccountRequest(context.Message.UserId);

        var result = await _accountContract.CreateVolunteerAccountAsync(request, CancellationToken.None);

        if (result.IsSuccess)
            _logger.LogInformation("Successfully created volunteer account with accountID = {ID}", result.Value);
        else
            _logger.LogError("Failed to create account for user with ID = {ID}", request.UserId);
    }
}