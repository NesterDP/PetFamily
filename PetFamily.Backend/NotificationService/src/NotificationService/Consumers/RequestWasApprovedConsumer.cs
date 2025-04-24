using MassTransit;
using NotificationService.Services;
using PetFamily.Accounts.Communication;
using PetFamily.VolunteerRequests.Contracts.Messaging;

namespace NotificationService.Consumers;

public class RequestWasApprovedConsumer : IConsumer<VolunteerRequestWasApprovedEvent>
{
    private readonly ILogger<RequestWasApprovedConsumer> _logger;
    private readonly IAccountsService _accountService;
    private readonly EmailService _emailService;

    public RequestWasApprovedConsumer(
        ILogger<RequestWasApprovedConsumer> logger,
        IAccountsService accountService,
        EmailService emailService)
    {
        _logger = logger;
        _accountService = accountService;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<VolunteerRequestWasApprovedEvent> context)
    {
        // получение данных от сервиса аккаунтов по http
        var userInfo = await _accountService
            .GetUserInfoById(context.Message.UserId, CancellationToken.None);

        if (userInfo.IsFailure)
            throw new Exception(userInfo.Error);

        var adminInfo = await _accountService
            .GetUserInfoById(context.Message.AdminId, CancellationToken.None);

        if (adminInfo.IsFailure)
            throw new Exception(userInfo.Error);

        // формирование и отправка сообщения на почту
        await _emailService.SendApprovedRequestNotification(
            userInfo.Value,
            adminInfo.Value,
            context.Message.RequestId);

        _logger.LogInformation(
            "Successfully notified user with ID = {Id} about approved request",
            context.Message.UserId);
    }
}