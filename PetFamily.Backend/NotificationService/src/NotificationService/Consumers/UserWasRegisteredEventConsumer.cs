using MassTransit;
using PetFamily.Accounts.Communication;
using PetFamily.Accounts.Contracts.Messaging;
using PetFamily.Accounts.Contracts.Requests;

namespace NotificationService.Consumers;

public class UserWasRegisteredEventConsumer : IConsumer<UserWasRegisteredEvent>
{
    private readonly ILogger<UserWasRegisteredEventConsumer> _logger;
    private readonly IAccountsHttpClient _httpClient;

    public UserWasRegisteredEventConsumer(
        ILogger<UserWasRegisteredEventConsumer> logger,
        IAccountsHttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task Consume(ConsumeContext<UserWasRegisteredEvent> context)
    {
        var userInfo = await _httpClient.GetUserInfoById(context.Message.UserId, CancellationToken.None);
        if (userInfo.IsFailure)
            throw new Exception(userInfo.Error);

        var request = new GenerateEmailTokenRequest(context.Message.UserId);
        var token = await _httpClient.GenerateEmailConfirmationToken(request, CancellationToken.None);
        if (token.IsFailure)
            throw new Exception(token.Error);

        // формирование и отправка сообщения на почту

        _logger.LogInformation("Successfully notified user with ID = {Id} about email confirmation",
            context.Message.UserId);
    }
}