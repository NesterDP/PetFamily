using Microsoft.Extensions.Options;
using NotificationService.Core.Options;
using PetFamily.Accounts.Communication;
using PetFamily.Accounts.Contracts.Dto;

namespace NotificationService.Services;

using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private const string NOTIFICATION_SERVICE_NAME = "Сервис уведомлений";
    private readonly AccountsServiceOptions _accountsOptions;
    private readonly SmtpOptions _smtpOptions;

    public EmailService(
        IOptions<SmtpOptions> smtpOptions,
        IOptions<AccountsServiceOptions> accountsOptions)
    {
        _accountsOptions = accountsOptions.Value;
        _smtpOptions = smtpOptions.Value;
    }

    public async Task SendConfirmationEmailAsync(UserInfoDto userInfo, string userId, string token)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(NOTIFICATION_SERVICE_NAME, _smtpOptions.FromEmail));
        message.To.Add(new MailboxAddress(string.Empty, userInfo.Email));
        message.Subject = "Подтверждение почты";

        string getLink = $"{_accountsOptions.Url}/{AccountsServiceConstants.EMAIL_CONFIRMATION_GET_ADDRESS}" +
                         $"?userId={userId}" +
                         $"&token={Uri.EscapeDataString(token)}";

        string postUrl = $"{_accountsOptions.Url}/{AccountsServiceConstants.EMAIL_CONFIRMATION_POST_ADDRESS}";

        var bodyBuilder = new BodyBuilder
        {
            // HTML-версия
            HtmlBody = $"""
                        <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                            <h2 style="color: #0066FF;">Подтверждение почты</h2>
                            
                            <!-- Основная кнопка (POST) -->
                            <form method="post" action="{postUrl}" style="margin-bottom: 25px;">
                                <input type="hidden" name="userId" value="{userId}">
                                <input type="hidden" name="token" value="{token}">
                                <button type="submit" style="
                                    background: #0066FF;
                                    color: white;
                                    padding: 12px 24px;
                                    border: none;
                                    border-radius: 6px;
                                    font-size: 16px;
                                    cursor: pointer;">
                                    Подтвердить почту
                                </button>
                            </form>
                            
                            <!-- Альтернативная ссылка (GET) -->
                            <p style="color: #555; font-size: 14px;">
                                Или нажмите <a href="{getLink}" style="color: #0066FF; text-decoration: none;">эту ссылку</a>
                            </p>
                        </div>
                        """,

            // Текстовая версия
            TextBody = $"Для завершения регистрации вы также скопировать эту ссылку" +
                       $" в адресную строку бразуера и перейти по ней:\n{getLink}"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
        await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendRevisionRequiredNotification(UserInfoDto userInfo, UserInfoDto adminInfo, Guid requestId)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(NOTIFICATION_SERVICE_NAME, _smtpOptions.FromEmail));

        message.To.Add(
            new MailboxAddress($"{userInfo.FullName.FirstName} {userInfo.FullName.LastName}", userInfo.Email));

        message.Subject = "Необходимость внесения изменений в заявку";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Админ {adminInfo.FullName.FirstName} {adminInfo.FullName.LastName} " +
                       $"проверил вашу заявку с ID = {requestId} и отправил её на доработку."
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
        await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendAmendedRequestNotification(UserInfoDto userInfo, UserInfoDto adminInfo, Guid requestId)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(NOTIFICATION_SERVICE_NAME, _smtpOptions.FromEmail));

        message.To.Add(
            new MailboxAddress($"{adminInfo.FullName.FirstName} {adminInfo.FullName.LastName}", adminInfo.Email));

        message.Subject = "Внесение изменений в заявку";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Пользователь {userInfo.FullName.FirstName} {userInfo.FullName.LastName} " +
                       $"изменил заявку с ID = {requestId} и отправил её на рассмотрение."
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
        await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendApprovedRequestNotification(UserInfoDto userInfo, UserInfoDto adminInfo, Guid requestId)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(NOTIFICATION_SERVICE_NAME, _smtpOptions.FromEmail));

        message.To.Add(
            new MailboxAddress($"{userInfo.FullName.FirstName} {userInfo.FullName.LastName}", userInfo.Email));

        message.Subject = "Одобрение заявки";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Админ {adminInfo.FullName.FirstName} {adminInfo.FullName.LastName} " +
                       $"проверил вашу заявку с ID = {requestId} и одобрил её."
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
        await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendRejectedRequestNotification(UserInfoDto userInfo, UserInfoDto adminInfo, Guid requestId)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(NOTIFICATION_SERVICE_NAME, _smtpOptions.FromEmail));

        message.To.Add(
            new MailboxAddress($"{userInfo.FullName.FirstName} {userInfo.FullName.LastName}", userInfo.Email));

        message.Subject = "Отклонение заявки";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Админ {adminInfo.FullName.FirstName} {adminInfo.FullName.LastName} " +
                       $"проверил вашу заявку с ID = {requestId} и отклонил её."
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
        await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendRequestWasTakenOnReviewNotification(
        UserInfoDto userInfo, UserInfoDto adminInfo, Guid requestId)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(NOTIFICATION_SERVICE_NAME, _smtpOptions.FromEmail));

        message.To.Add(
            new MailboxAddress($"{userInfo.FullName.FirstName} {userInfo.FullName.LastName}", userInfo.Email));

        message.Subject = "Взятие заявки на рассмотрение";

        var bodyBuilder = new BodyBuilder
        {
            TextBody = $"Админ {adminInfo.FullName.FirstName} {adminInfo.FullName.LastName} " +
                       $"начал проверять вашу заявку с ID = {requestId} ."
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
        await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}