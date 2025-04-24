using MassTransit;
using NotificationService.Services;
using PetFamily.Accounts.Communication;
using PetFamily.Accounts.Contracts.Messaging;
using PetFamily.Accounts.Contracts.Requests;

namespace NotificationService.Consumers;

public class UserWasRegisteredEventConsumer : IConsumer<UserWasRegisteredEvent>
{
    private readonly ILogger<UserWasRegisteredEventConsumer> _logger;
    private readonly IAccountsService _accountService;
    private readonly EmailService _emailService;

    public UserWasRegisteredEventConsumer(
        ILogger<UserWasRegisteredEventConsumer> logger,
        IAccountsService accountService,
        EmailService emailService)
    {
        _logger = logger;
        _accountService = accountService;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserWasRegisteredEvent> context)
    {
        // получение данных от сервиса аккаунтов по http
        var userInfo = await _accountService.GetUserInfoById(context.Message.UserId, CancellationToken.None);
        if (userInfo.IsFailure)
            throw new Exception(userInfo.Error);

        var request = new GenerateEmailTokenRequest(context.Message.UserId);
        var token = await _accountService.GenerateEmailConfirmationToken(request, CancellationToken.None);
        if (token.IsFailure)
            throw new Exception(token.Error);

        // формирование и отправка сообщения на почту
        await _emailService.SendConfirmationEmailAsync(userInfo.Value, context.Message.UserId.ToString(), token.Value);

        _logger.LogInformation(
            "Successfully notified user with ID = {Id} about email confirmation",
            context.Message.UserId);
    }
}