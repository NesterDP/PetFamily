using MassTransit;
using NotificationService.Services;
using PetFamily.Accounts.Communication;
using PetFamily.VolunteerRequests.Contracts.Messaging;

namespace NotificationService.Consumers;

public class RequestWasTakenOnReviewConsumer : IConsumer<VolunteerRequestWasTakenOnReviewEvent>
{
    private readonly ILogger<RequestWasTakenOnReviewConsumer> _logger;
    private readonly IAccountsService _accountService;
    private readonly EmailService _emailService;

    public RequestWasTakenOnReviewConsumer(
        ILogger<RequestWasTakenOnReviewConsumer> logger,
        IAccountsService accountService,
        EmailService emailService)
    {
        _logger = logger;
        _accountService = accountService;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<VolunteerRequestWasTakenOnReviewEvent> context)
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
        await _emailService.SendRequestWasTakenOnReviewNotification(
            userInfo.Value,
            adminInfo.Value,
            context.Message.RequestId);

        _logger.LogInformation("Successfully notified user with ID = {Id} that his request was taken on review",
            context.Message.UserId);
    }
}