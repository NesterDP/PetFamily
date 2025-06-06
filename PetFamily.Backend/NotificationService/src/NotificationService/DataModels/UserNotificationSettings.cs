using NotificationService.DataModels.ValueObjects;

namespace NotificationService.DataModels;

public class UserNotificationSettings
{
    public UserNotificationSettingsId Id { get; private set; } = null!;

    public UserId UserId { get; private set; } = null!;

    public bool? SendToEmail { get; private set; }

    public bool? SendToTelegram { get; private set; }

    public bool SendToWebsite { get; private set; }

    public UserNotificationSettings(
        UserId userId,
        bool? sendToEmail,
        bool? sendToTelegram,
        bool sendToWebsite)
    {
        Id = UserNotificationSettingsId.NewId();
        UserId = userId;
        SendToEmail = sendToEmail;
        SendToTelegram = sendToTelegram;
        SendToWebsite = sendToWebsite;
    }

    public void SetUserId(UserId userId)
    {
        UserId = userId;
    }

    public void SetEmailNotification(bool? sendToEmail)
    {
        SendToEmail = sendToEmail;
    }

    public void SetTelegramNotification(bool? sendToTelegram)
    {
        SendToTelegram = sendToTelegram;
    }

    public void SetWebSiteNotification(bool sendToWebsite)
    {
        SendToWebsite = sendToWebsite;
    }

    // ef core
    // ReSharper disable once UnusedMember.Local
    private UserNotificationSettings() { }
}